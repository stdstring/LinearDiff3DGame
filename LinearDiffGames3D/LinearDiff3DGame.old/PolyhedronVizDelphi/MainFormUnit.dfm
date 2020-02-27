object MainForm: TMainForm
  Left = 193
  Top = 108
  Width = 783
  Height = 540
  Caption = 'MainForm'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  Menu = MainMenu1
  OldCreateOrder = False
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  OnKeyDown = FormKeyDown
  OnKeyUp = FormKeyUp
  OnResize = FormResize
  PixelsPerInch = 96
  TextHeight = 13
  object MainMenu1: TMainMenu
    Left = 16
    Top = 464
    object miActions: TMenuItem
      Caption = 'Actions'
      object miOpenDataFile: TMenuItem
        Caption = 'Open data'#39's file'
        OnClick = miOpenDataFileClick
      end
      object miExit: TMenuItem
        Caption = 'Exit'
        OnClick = miExitClick
      end
    end
    object miVizualizationParameters: TMenuItem
      Caption = 'Visualization parameters'
      object miShowEdges: TMenuItem
        Caption = 'Show edges'
        OnClick = miShowEdgesClick
      end
      object miKeepViewParameters: TMenuItem
        Caption = 'Keep view parameters'
        OnClick = miKeepViewParametersClick
      end
    end
  end
  object OpenDialog1: TOpenDialog
    Filter = '*.dat'
    Left = 56
    Top = 464
  end
end
