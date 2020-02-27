unit MainFormUnit;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, OpenGL, Menus;

type
  TVector3D = record
    X, Y, Z: GLfloat;
  end;

  TPoint3D = record
    X, Y, Z: GLfloat;
  end;

  TSide = record
    Vertexes: array of TPoint3D;
  end;

  EIcorrectDataFileFormat = class(Exception)
  end;

  TMainForm = class(TForm)
    MainMenu1: TMainMenu;
    miActions: TMenuItem;
    miExit: TMenuItem;
    OpenDialog1: TOpenDialog;
    miOpenDataFile: TMenuItem;
    miVizualizationParameters: TMenuItem;
    miShowEdges: TMenuItem;
    miKeepViewParameters: TMenuItem;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure miExitClick(Sender: TObject);
    procedure FormResize(Sender: TObject);
    procedure FormKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure FormKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure miOpenDataFileClick(Sender: TObject);
    procedure miShowEdgesClick(Sender: TObject);
    procedure miKeepViewParametersClick(Sender: TObject);
  private
    { Private declarations }
    m_DC:HDC;
    m_hrc:HGLRC;

    m_NormalsList: TList;
    m_SidesList: TList;

    // угол поворота относительно оси OY (по горизонтали)
    m_AngleOY: GLfloat;
    // скорость приращения угла m_AngleOY (рад/с)
    m_StepAngleOYVelocity: GLfloat;
    // угол поворота относительно оси OX (по вертикали)
    m_AngleOX: GLfloat;
    // скорость приращения угла m_AngleOX (рад/с)
    m_StepAngleOXVelocity: GLfloat;
    // ???
    m_ViewAngle: GLFloat;

    //
    m_LastTickCount: longint;
    m_CurrentTickCount: longint;
    m_PrevTickCount: longint;
    m_FrameCount: longint;

    m_ShowEdges: boolean;
    m_KeepViewParameters: boolean;

    procedure SplitStr(SourceStr: string; DelimChar: char; DestStringList: TStringList);
    procedure LoadPolyhedronDataFromFile(PolyhedronDataFileName: string);

    procedure SetDCPixelFormat();

    procedure OpenGLInit();
    procedure OpenGLFinalize();

    procedure PolyhedronInit(PolyhedronDataFileName: string);
    procedure PolyhedronFinalize();
  protected
    procedure WMPaint(var Msg: TWMPaint);message WM_PAINT;
  public
    { Public declarations }
  end;

var MainForm: TMainForm;

implementation

{$R *.dfm}

const DATA_STR_DELIMITER = ' ';
      POLYHEDRON_SURFACE = 1;
      POLYHEDRON_EDGES = 2;
      MAX_STEP_ANGLE_OYVEL = 30;
      MAX_STEP_ANGLE_OXVEL = 30;
      START_VIEWANGLE = 40;
      MAX_STEP_VIEWANGLE_VEL = 2;

// метод SplitStr разделяет строку на подстроки (DelimChar - символ, отделяющий одну подстроку от другой в исходной строке)
// и возвращает набор подстрок в списке DestStringList
procedure TMainForm.SplitStr(SourceStr: string; DelimChar:char; DestStringList: TStringList);

var SourceStrLen: integer;
    CharIndex: integer;
    PrevDelimCharIndex: integer;

begin
  SourceStrLen:=Length(SourceStr);
  PrevDelimCharIndex:=0;

  for CharIndex:=1 to SourceStrLen do
    begin
      if (SourceStr[CharIndex] = DelimChar) then
        begin
          DestStringList.Add(Copy(SourceStr, PrevDelimCharIndex+1, CharIndex-PrevDelimCharIndex-1));
          PrevDelimCharIndex:=CharIndex;
        end;
    end;

  DestStringList.Add(Copy(SourceStr, PrevDelimCharIndex+1, SourceStrLen-PrevDelimCharIndex));
end;

// метод LoadPolyhedronDataFromFile загружает структуру многогранника из файла с именем PolyhedronDataFileName
// формат файла с данными:
// ...
// nx ny nz - координаты нормали i-й грани, разделенные пробелом (в одной строке)
// v1x v1y v1z ... vnx vny vnz - координаты вершин i-й грани, разделенные пробелом (в одной строке)
// ...
// ВАЖНО: вершины грани должны быть упорядочены против ч.с. (если смотреть на грань с конца внешней нормали)
procedure TMainForm.LoadPolyhedronDataFromFile(PolyhedronDataFileName: string);

var PolyhedronDataFile: TextFile;
    CurrentDataStr: string;
    CurrentDataStrParts: TStringList;
    pCurrentNormalVector: ^TVector3D;
    PartsIndex: integer;
    pCurrentSide: ^TSide;
    DecimalSeparator: char;
    FormatSettings: TFormatSettings;

begin
  CurrentDataStrParts:=nil;

  GetLocaleFormatSettings(0, FormatSettings);
  DecimalSeparator:=' ';

  try
    CurrentDataStrParts:=TStringList.Create();

    AssignFile(PolyhedronDataFile, PolyhedronDataFileName);
    Reset(PolyhedronDataFile);

    while not eof(PolyhedronDataFile) do
      begin
        // читаем и обрабатываем строку с координатами нормали i-й грани
        Readln(PolyhedronDataFile, CurrentDataStr);

        if (DecimalSeparator = ' ') then
          begin
            if (Pos(',', CurrentDataStr) > 0) then
              begin
                DecimalSeparator:=',';
                FormatSettings.DecimalSeparator:=',';
              end;
            if (Pos('.', CurrentDataStr) > 0) then
              begin
                DecimalSeparator:='.';
                FormatSettings.DecimalSeparator:='.';
              end;
          end;

        CurrentDataStrParts.Clear();
        SplitStr(CurrentDataStr, DATA_STR_DELIMITER, CurrentDataStrParts);

        if (CurrentDataStrParts.Count <> 3) then
          begin
            raise EIcorrectDataFileFormat.Create('Incorrect format of polyhedron"s datafile');
          end;

        New(pCurrentNormalVector);

        pCurrentNormalVector^.X:=StrToFloat(CurrentDataStrParts[0], FormatSettings);
        pCurrentNormalVector^.Y:=StrToFloat(CurrentDataStrParts[1], FormatSettings);
        pCurrentNormalVector^.Z:=StrToFloat(CurrentDataStrParts[2], FormatSettings);

        // читаем и обрабатываем строку с координатами вершин i-й грани
        Readln(PolyhedronDataFile, CurrentDataStr);

        if (DecimalSeparator = ' ') then
          begin
            if (Pos(',', CurrentDataStr) > 0) then
              begin
                DecimalSeparator:=',';
                FormatSettings.DecimalSeparator:=',';
              end;
            if (Pos('.', CurrentDataStr) > 0) then
              begin
                DecimalSeparator:='.';
                FormatSettings.DecimalSeparator:='.';
              end;
          end;

        CurrentDataStrParts.Clear();
        SplitStr(CurrentDataStr, DATA_STR_DELIMITER, CurrentDataStrParts);

        if ((CurrentDataStrParts.Count mod 3) <> 0) then
          begin
            raise EIcorrectDataFileFormat.Create('Incorrect format of polyhedron"s datafile');
          end;

        New(pCurrentSide);
        SetLength(pCurrentSide^.Vertexes, (CurrentDataStrParts.Count div 3));

        for PartsIndex:=0 to (CurrentDataStrParts.Count div 3)-1 do
          begin
            pCurrentSide^.Vertexes[PartsIndex].X:=StrToFloat(CurrentDataStrParts[PartsIndex*3+0], FormatSettings);
            pCurrentSide^.Vertexes[PartsIndex].Y:=StrToFloat(CurrentDataStrParts[PartsIndex*3+1], FormatSettings);
            pCurrentSide^.Vertexes[PartsIndex].Z:=StrToFloat(CurrentDataStrParts[PartsIndex*3+2], FormatSettings);
          end;

        m_NormalsList.Add(pCurrentNormalVector);
        m_SidesList.Add(pCurrentSide);
      end;

  finally
    CloseFile(PolyhedronDataFile);
    CurrentDataStrParts.Free();
  end;
end;

//
procedure TMainForm.SetDCPixelFormat();

var nPixelFormat: integer;
    pfd: TPixelFormatDescriptor;

begin
  FillChar(pfd, SizeOf(pfd), 0);

  //
  pfd.dwFlags:=PFD_DRAW_TO_WINDOW or PFD_SUPPORT_OPENGL or PFD_DOUBLEBUFFER;
  //
  nPixelFormat:=ChoosePixelFormat(m_DC, @pfd);
  //
  SetPixelFormat(m_DC, nPixelFormat, @pfd);
end;

//
procedure TMainForm.OpenGLInit();

begin
  //
  m_DC:=GetDC(Handle);
  //
  SetDCPixelFormat();
  //
  m_hrc:=wglCreateContext(m_DC);
  //
  wglMakeCurrent(m_DC, m_hrc);
  // цвет фона (возможно, что эта команда должна распологаться не здесь)
  glClearColor(1.0, 1.0, 1.0, 1.0);

  // from procedure Init() ???
  //
  glEnable(GL_DEPTH_TEST);
  //
  glEnable(GL_LIGHTING);
  //
  glEnable(GL_LIGHT0);
  //
  glEnable (GL_COLOR_MATERIAL);
  //
  glLightModeli(GL_LIGHT_MODEL_TWO_SIDE, 1);
  // цвет многогранника (возможно, что эта команда должна распологаться не здесь)
  // glColor3f (0.4, 0.6, 0.6);
  // ???
  {glPolygonMode(GL_FRONT, GL_LINE);
  glEnable(GL_CULL_FACE);
  glCullFace(GL_BACK);}
  //
  glLineWidth(3.0);
end;

//
procedure TMainForm.OpenGLFinalize();

begin
  //
  wglMakeCurrent(0, 0);
  //
  wglDeleteContext(m_hrc);
  //
  ReleaseDC(Handle, m_DC);
  //
  DeleteDC(m_DC);
end;

//
procedure TMainForm.PolyhedronInit(PolyhedronDataFileName: string);

var SideIndex, VertexIndex: integer;
    pCurrentNormal: ^TVector3D;
    pCurrentSide: ^TSide;

begin
  LoadPolyhedronDataFromFile(PolyhedronDataFileName);

  //
  glNewList(POLYHEDRON_SURFACE, GL_COMPILE);
  for SideIndex:=0 to m_SidesList.Count-1 do
    begin
      pCurrentNormal:=m_NormalsList[SideIndex];
      pCurrentSide:=m_SidesList[SideIndex];

      for VertexIndex:=1 to Length(pCurrentSide^.Vertexes)-2 do
        begin
          //
          glBegin(GL_TRIANGLES);
          //
          glNormal3fv(@pCurrentNormal^);
          //
          glvertex3fv(@pCurrentSide^.Vertexes[0]);
          glvertex3fv(@pCurrentSide^.Vertexes[VertexIndex]);
          glvertex3fv(@pCurrentSide^.Vertexes[VertexIndex+1]);
          //
          glEnd();
        end;
    end;
  //
  glEndList();

  //
  glNewList(POLYHEDRON_EDGES, GL_COMPILE);
  for SideIndex:=0 to m_SidesList.Count-1 do
    begin
      pCurrentNormal:=m_NormalsList[SideIndex];
      pCurrentSide:=m_SidesList[SideIndex];

      //
      glBegin(GL_POLYGON);
      //
      glNormal3fv(@pCurrentNormal^);
      for VertexIndex:=0 to Length(pCurrentSide^.Vertexes)-1 do
        begin
          //
          glvertex3fv(@pCurrentSide^.Vertexes[VertexIndex]);
        end;
      glEnd();
    end;
  //
  glEndList();

  if not(m_KeepViewParameters) then
    begin
      m_AngleOY:=0;
      m_StepAngleOYVelocity:=0;
      m_AngleOX:=0;
      m_StepAngleOXVelocity:=0;
      m_ViewAngle:=START_VIEWANGLE;
    end;

  FormResize(nil);
end;

//
procedure TMainForm.PolyhedronFinalize();

var SideIndex: integer;

begin
  //
  glDeleteLists(POLYHEDRON_SURFACE, 1);

  for SideIndex:=0 to m_SidesList.Count-1 do
    begin
      Dispose(m_NormalsList[SideIndex]);
      Dispose(m_SidesList[SideIndex]);
    end;

  m_NormalsList.Clear();
  m_SidesList.Clear();      
end;

//
procedure TMainForm.WMPaint(var Msg: TWMPaint);

var ps: TPaintStruct;
    FPSValue: GLfloat;

begin
  //
  BeginPaint(Handle, ps);
  //
  glClear (GL_COLOR_BUFFER_BIT or GL_DEPTH_BUFFER_BIT);
  //
  glPushMatrix();

  // поворот относительно оси OY
  glRotatef(m_AngleOY, 0.0, 1.0, 0.0);
  // поворот относительно оси OX
  glRotatef(m_AngleOX, 1.0, 0.0, 0.0);

  //
  glColor3f (0.4, 0.6, 0.6);
  //
  glCallList(POLYHEDRON_SURFACE);

  if (m_ShowEdges) then
    begin
      //
      glColor3f (0.0, 0.0, 0.0);
      //
      glPolygonMode(GL_FRONT, GL_LINE);
      //
      glEnable(GL_CULL_FACE);
      //
      glCullFace(GL_BACK);
      //
      glCallList(POLYHEDRON_EDGES);
      //
      glPolygonMode(GL_FRONT, GL_FILL);
      //
      glDisable(GL_CULL_FACE);
    end;

  //
  glPopMatrix();
  //
  SwapBuffers(m_DC);
  //
  EndPaint(Handle, ps);

  m_PrevTickCount:=m_CurrentTickCount;
  m_CurrentTickCount:=GetTickCount();
  Inc(m_FrameCount);

  //
  if (m_StepAngleOYVelocity<>0) then
    begin
      m_AngleOY:=m_AngleOY+m_StepAngleOYVelocity*(m_CurrentTickCount-m_PrevTickCount)/1000;
      if (m_AngleOY >= 360.0) then m_AngleOY:=m_AngleOY-360.0;
      if (m_AngleOY < 0) then m_AngleOY:=360.0+m_AngleOY;
    end;
  //
  if (m_StepAngleOXVelocity<>0) then
    begin
      m_AngleOX:=m_AngleOX+m_StepAngleOXVelocity*(m_CurrentTickCount-m_PrevTickCount)/1000;
      if (m_AngleOX >= 360.0) then m_AngleOX:=m_AngleOX-360.0;
      if (m_AngleOX < 0) then m_AngleOX:=360.0+m_AngleOX;
    end;
  //
  if ((m_CurrentTickCount-m_LastTickCount)>=1000) then
    begin
      FPSValue:=m_FrameCount*1000/(m_CurrentTickCount-m_LastTickCount);
      Caption:='MainForm. FPS = '+FloatToStr(FPSValue);
      m_LastTickCount:=m_CurrentTickCount;
      m_FrameCount:=0;
    end;
  
  InvalidateRect(Handle, nil, False);
end;

procedure TMainForm.FormCreate(Sender: TObject);
begin
  m_NormalsList:=TList.Create();
  m_SidesList:=TList.Create();

  OpenGLInit();

  m_AngleOY:=0;
  m_StepAngleOYVelocity:=0;
  m_AngleOX:=0;
  m_StepAngleOXVelocity:=0;
  m_ViewAngle:=START_VIEWANGLE;
  
  m_LastTickCount:=GetTickCount();
  m_CurrentTickCount:=m_LastTickCount;
  m_FrameCount:=0;

  m_ShowEdges:=false;
  m_KeepViewParameters:=false;
end;

procedure TMainForm.FormResize(Sender: TObject);
begin
  //
  glViewPort (0, 0, ClientWidth, ClientHeight);
  //
  glMatrixMode(GL_PROJECTION);
  //
  glLoadIdentity;
  //
  gluPerspective(m_ViewAngle, ClientWidth / ClientHeight, 3.0, 13.0);
  //
  glMatrixMode(GL_MODELVIEW);
  //
  glLoadIdentity;
  //
  glTranslatef(0.0, 0.0, -9.0);
  
  InvalidateRect(Handle, nil, False);
end;

procedure TMainForm.FormDestroy(Sender: TObject);
begin
  PolyhedronFinalize();
  OpenGLFinalize();

  m_NormalsList.Free();
  m_SidesList.Free();
end;

procedure TMainForm.FormKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if (Key=VK_LEFT) then
    begin
      m_StepAngleOYVelocity:=-MAX_STEP_ANGLE_OYVEL;
      InvalidateRect(Handle, nil, False);
    end;
  if (Key=VK_RIGHT) then
    begin
      m_StepAngleOYVelocity:=MAX_STEP_ANGLE_OYVEL;
      InvalidateRect(Handle, nil, False);
    end;
  if (Key=VK_UP) then
    begin
      m_StepAngleOXVelocity:=-MAX_STEP_ANGLE_OXVEL;
      InvalidateRect(Handle, nil, False);
    end;
  if (Key=VK_DOWN) then
    begin
      m_StepAngleOXVelocity:=MAX_STEP_ANGLE_OXVEL;
      InvalidateRect(Handle, nil, False);
    end;
  if (Key=VK_ADD) then
    begin
      //m_ViewAngle:=m_ViewAngle+MaxStepViewAngleVelocity*(GetTickCount()-m_PrevTickCount)/1000;
      m_ViewAngle:=m_ViewAngle-MAX_STEP_VIEWANGLE_VEL;
      FormResize(nil);
    end;
  if (Key=VK_SUBTRACT) then
    begin
      //m_ViewAngle:=m_ViewAngle-MaxStepViewAngleVelocity*(GetTickCount()-m_PrevTickCount)/1000;
      m_ViewAngle:=m_ViewAngle+MAX_STEP_VIEWANGLE_VEL;
      FormResize(nil);
    end;
end;

procedure TMainForm.FormKeyUp(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if ((Key=VK_LEFT) or (Key=VK_RIGHT)) then
    begin
      m_StepAngleOYVelocity:=0;
      InvalidateRect(Handle, nil, False);
    end;
  if ((Key=VK_UP) or (Key=VK_DOWN)) then
    begin
      m_StepAngleOXVelocity:=0;
      InvalidateRect(Handle, nil, False);
    end;
end;

procedure TMainForm.miOpenDataFileClick(Sender: TObject);
begin
  if (OpenDialog1.Execute()) then
    begin
      PolyhedronFinalize();
      PolyhedronInit(OpenDialog1.FileName);
    end;
end;

procedure TMainForm.miExitClick(Sender: TObject);
begin
  Close();
end;

procedure TMainForm.miShowEdgesClick(Sender: TObject);
begin
  m_ShowEdges:=not m_ShowEdges;
  miShowEdges.Checked:=m_ShowEdges;

  InvalidateRect(Handle, nil, False);
end;

procedure TMainForm.miKeepViewParametersClick(Sender: TObject);
begin
  m_KeepViewParameters:=not m_KeepViewParameters;
  miKeepViewParameters.Checked:=m_KeepViewParameters;
end;

end.
