   M S B A M L   `   `   `  ���� H  DBdatumRestore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:BdatumRestore.ViewModelBdatumRestore.ViewModel  A%clr-namespace:BdatumRestore.ViewModelBdatumRestore.ViewModel  #    BdatumRestore.View.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     7local%clr-namespace:BdatumRestore.ViewModel     Hi:http://schemas.microsoft.com/expression/2010/interactivity     4vm%clr-namespace:BdatumRestore.ViewModel     -     � Title$  B-Datum Restore��$	��642��$	��950�� � Icon$8 2/BdatumRestore;component/Images/favicon_bdatum.ico�� � 
ResizeMode$ CanMinimize=� J� 	AllowDrop . $��0q� � WindowStyle$ SingleBorderWindow=����	#FFDDDDDD��Z� .��� -   $
��Grid��$��Stretch=�$��Stretch=����	#FFDDDDDD$	��603��$	��928��.+�R� -   $��	TreeView1�� .  
�������$
��Left=�$��148,170,0,172q�$	��313��
���   � J� Opacity$ 1Q�|�.  $��1q����� *	Folders+����  �� 
TargetType" Q�.���  �� Property � |� �� Value .  ��  �@Q�
IsExpanded �� 	 �� Path$	 
isExpanded�
 �� UpdateSourceTrigger$
 PropertyChanged=�����  �� DataType%   BdatumRestore.ViewModel.IFolder"   �� ItemsSource �� *	Folders+.R��� $��ButtonItems�� �� 	ClickMode$ Press=�|�.  $��20��$��0q���Z� ��Z� ����  �� ElementName$ button2��$	 Command�����  � ToolTip �� $	 
FolderName� �� Mode$ OneTime=�.���� $��0q� �� Orientation$ 
Horizontal=�$��Stretch=�.+��� $
��Fill=����� ,   &BdatumRestore.ViewModel.ImageConverter   Instance �� 	Converter# Z $ OneTime=��� $��ItemText��$��5,0,0,0q����� *
FolderName+$ OneTime=� � 	Resources  �� RelativeSource �� AncestorType��%�   'Q�       �� " Q�.���  � �� �� .R��� $ 
Horizontal=�.+��� $	��img��$��20��$
��Fill=�$��20������  �� *FindAncestor+" Q�$	 Header�# Z �� $	��5,0q����� �� -   $��RestoreButton�� .  $��	Restaurar��$��23��$
��Left=�$��615,448,0,0q�$	��Top=�$��75��$��0q����� *DownloadCommand+��Z� ����  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ 	#FF0096FF�� � Offset$
 0.25Q�� $ 	#FF1569A0��$ 0.814Q��� -   $��button2�� .  $�� ��$��23��$
��Left=�$��370,445,0,0q�$	��Top=�$��75��$y�Hidden=����� *UpdateCommand+�� -   $��
textBlock1�� .  $��24(�$��35��$
��Left=�$��253,131,0,0q�$ 5Q�$��Diretórios��$	��Top=��� -   $��
textBlock2�� .  $��24(�$��35��$
��Left=�$��613,131,0,0q�$ 5Q�$��Arquivos��$	��Top=��� -   $��image1��$	��101��$��60,26,708,0q�$
��Fill=�$	��Top=�$5��//BdatumRestore;component/Images/logo_bdatum.png���� -	   $��label1��$)��#Selecione uma pasta ou os arquivos.��$��53��$��281,26,139,0q�$	��Top=�$��26(���
� .��� $ Black��$ 0Q�� $ Black��$ 1Q��� -
   $��label2��$��para fazer restore.��$��56��$
��Left=�$��385,0,0,481q�$��Bottom=�$��26(�$	��224����
� .��� $ Black��$ 0Q�� $ Black��$ 1Q��� -   $��SelectedDir��$��28��$
��Left=�$��379,131,0,0q�$	��Top=�$	��211�����	#FFD11515$��Center=����� *FolderSelected+� -   $��progressBar1��$��19��$
��Left=�$��236,517,0,0q�$	��Top=�$	��506��
���   ����� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF040C17��$ 0.924Q�� $ 	#FF111114��$ 0.436Q�� $ Black��$ 0.192Q�� $ 	#FF1D1D1D��$ 0.628Q��� -   $��ProgressLabel��
���   �$��28��$
��Left=�$��236,485,0,0q�$	��Top=�$	��506��$��Progresso do download��$��Center=��� -   $��PauseButton�� .  $��0q�$��Pausar��$��23��$
��Left=�$��257,448,0,0q�$	��Top=�$��75����Z� ���� *PauseCommand+���� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF0096FF��$
 0.25Q�� $ 	#FF1569A0��$ 0.814Q��� -   $��border1�����	#FF0D0D0D$��1q�$��84��$
��Left=�$��503,0,0,172q�$��Bottom=�$	��316�� �� CornerRadius$ 5��
�������._��  � RowDefinitions �� $	��25* ��� $	��57* �.+��� -   $��FileDate��$�� ��$��27��$
��Left=�$��6,6,0,0q�$	��Top=�$	��302��$��12(�$��2���� -   $��FileType��$�� ��$��12(�$��24��$
��Left=�$��6,3,0,0q�$	��Top=�$	��302��$��1���� -   $��FileSize��$�� ��$��12(�$��26��$��6,0,6,3q�$��Bottom=�$��1���� -   $��VersionButton��$��Versões��$��23��$
��Left=�$��	228,8,0,0q�$	��Top=�$��75��{�.  J� IsHitTestVisible . $y�Visible=�$��2������ *VersionCommand+�� -   $��
BackButton��$��Voltar��$��23��$
��Left=�{�.  . $��
228,28,0,0q�$	��Top=�$y�Visible=�$��75��$��1������ *UpdateCommand+�� -   $��ResumeButton�� .  $��0q�$��	Continuar��$��23��$
��Left=�$��443,448,0,0q�$	��Top=�$��75����Z� ���� *ResumeCommand+���� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF0096FF��$
 0.25Q�� $ 	#FF1569A0��$ 0.814Q��� -   $��ProgressCountLabel��
���   �$��28��$
��Left=�$��Center=�$��236,542,0,0q�$	��Top=�$	��506���� -   $��FileList�� .  
�������$��504,170,108,261q� �� SelectionMode$ Extended=�$��1q�
���   �$ 1Q�|�.  $	��316������ *Files+ �� View �� .P���   �� Width$	  157��$
��Nome��! �� DisplayMemberBinding! �� *
FileName+�� $	  157��$��Tamanho��! �� *Size+�� -   $��FilesExistLabel��$��{FilesExistLabel}��$��28��$
��Left=�$��504,200,0,0q�$	��Top=����	#FFFC0000$	��316��$��Center=�9� -   $��DetailExpander��$��Detalhes��$	��100��$
��Left=�$��	9,495,0,0q�$	��Top=�$	��225��$��Center=�.���� -   $��DownloadDetails�� .  
�������$ Single=�$��1q�
���   �$ 1Q�|�.  $	��220������ *
DetailList+ �� .P��� $	  110��$��Arquivo��! �� *
FileName+�� $	  110��$��Status��! �� *Status+�� -   $��LoadingLabel��$��Carregando Diretórios...�����	#FFFC0000$��28��$
��Left=�$��Center=�$��148,200,0,0q�$	��Top=�$	��313��