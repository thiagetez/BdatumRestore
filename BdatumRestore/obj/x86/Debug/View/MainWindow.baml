   M S B A M L   `   `   `  ����H  DBdatumRestore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:BdatumRestore.ViewModelBdatumRestore.ViewModel  5   	   A%clr-namespace:BdatumRestore.ViewModelBdatumRestore.ViewModel  5   	   #    BdatumRestore.View.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   7local%clr-namespace:BdatumRestore.ViewModel     5   	   Hi:http://schemas.microsoft.com/expression/2010/interactivity     5   	   4vm%clr-namespace:BdatumRestore.ViewModel     5   	   -   5          � Title$  B-Datum Restore��5   	   $	��642��6!   $	��950��6.    � Icon$8 2/BdatumRestore;component/Images/favicon_bdatum.ico��6:    � 
ResizeMode$ CanMinimize=�6t    J� 	AllowDrop . $��0q�6�    � WindowStyle$ SingleBorderWindow=�6�   ���	#FFDDDDDD��6�   Z� .��5	      � -   5        $
��Grid��5	   E   $��Stretch=�6   $��Stretch=�6)   ���	#FFDDDDDD$	��603��6h   $	��928��6u   .+�5
   
   R� -   5        $��	TreeView1��5
   �    .  
�������$
��Left=�68   $��148,170,0,172q�6s   $	��313��6�   
���   � J� Opacity$ 1Q�6�   |�.  $��1q�6�   ��6S   �� *	Folders+��5      �� 5       �� 
TargetType" Q�6   .�5      ��  �� Property � |� �� Value .  62   �� 5       �@Q�
IsExpanded 63   �� 	 �� Path$	 
isExpanded�
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5      ��5      �� 5       �� DataType%   BdatumRestore.ViewModel.IFolder"  6+    �� ItemsSource 6J   �� *	Folders+.R�5      �� $��ButtonItems��6�    �� 	ClickMode$ Press=�6J   |�.  $��20��6�   $��0q�6�   ��6   Z� ��63   Z� ��6]   ��  �� ElementName$ button2��$	 Command���6�   ��  � ToolTip 6�   �� $	 
FolderName�$
 OneTime=�.��5      �� $��0q�6%    �� Orientation$ 
Horizontal=�60   $��Stretch=�6I   .+�5      �� $
��Fill=�6u   ��6$   �� ,   &BdatumRestore.ViewModel.ImageConverter   Instance �� 	Converter# Z $
 OneTime=�6u   �� 5      $��ItemText��69   $��5,0,0,0q�6(   ��6I   �� *
FolderName+$
 OneTime=�5      5      5       � 	Resources 5       �� RelativeSource �� AncestorType��%^  'Q�       �� 5      " Q�6   .�5      ��  � �� 5       �� 5!      .R�5"   "   �� $ 
Horizontal=�6-   .+�5#   &   �� $	��img��68   $��20��6,   $
��Fill=�6�   $��20��6�   ��6C   ��  �� *FindAncestor+" Q�$	 Header�# Z 6�   �� 5$   &   $	��5,0q�60   ��6=   �� 5%   #   5&      5(      5)      5*      5+      �� 5,   
   -   5        $��RestoreButton��5,   �    .  $��	Restaurar��6G   $��23��6[   $
��Left=�6g   $��615,448,0,0q�6�   $	��Top=�6�   $��75��6�   $��0q�6�   ��6#   �� *DownloadCommand+��6�   Z� ��5-      �� 5.       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5/      �  � Color$ 	#FF0096FF��6#    � Offset$
 0.25Q�65   � 50      $ 	#FF1569A0��6#   $ 0.814Q�65   51      53      �� 54   
   -   5        $��button2��54   �    .  $�� ��6E   $��23��6P   $
��Left=�6\   $��370,445,0,0q�6w   $	��Top=�6�   $��75��6�   $y�Hidden=�6�   ��6#   �� *UpdateCommand+6�   �� 55   
   -   5        $��
textBlock1��55   p    .  $��24(�6&   $��35��64   $
��Left=�6@   $��249,131,0,0q�6[   $ 5Q�6�   $��Diretórios��6�   $	��Top=�6�   �� 56   
   -   5        $��
textBlock2��56   p    .  $��24(�6&   $��35��64   $
��Left=�6@   $��613,131,0,0q�6[   $ 5Q�6�   $��Arquivos��6�   $	��Top=�6�   �� 57   
   -   5        $��image1��57   2   $	��101��6   $��60,26,708,0q�6   $
��Fill=�6@   $	��Top=�6O   $5��//BdatumRestore;component/Images/logo_bdatum.png��6g   �� 58   
   -	   5        $��label1��58   `   $)��#Selecione uma pasta ou os arquivos.��6   $��53��6>   $��281,26,139,0q�6J   $	��Top=�6n   $��26(�6�   ��59      
� 5:      .��5;      � $ Black��6#   $ 0Q�61   � 5<      $ Black��6#   $ 1Q�61   5=      5?      �� 5@   
   -
   5        $��label2��5@   j   $��para fazer restore.��6   $��56��6.   $
��Left=�6:   $��385,0,0,481q�6U   $��Bottom=�6x   $��26(�6�   $	��224��6�   ��5A      
� 5B      .��5C      � $ Black��6#   $ 0Q�61   � 5D      $ Black��6#   $ 1Q�61   5E      5G      �� 5H   
   -   5        $��SelectedDir��5H   M   $��28��6   $
��Left=�6   $��379,131,0,0q�68   $	��Top=�6�   $	��211��6�   ���	#FFD11515$��Center=�6�   ��6`   �� *FolderSelected+6�   � 5I   
   -   5        $��progressBar1��5I   R   $��19��6   $
��Left=�6"   $��236,517,0,0q�6=   $	��Top=�6f   $	��506��6~   
���   ���5J      �� 5K      $ 0.5,1&�6&   $ 0.5,0&�67   .��5L      � $ 	#FF040C17��6#   $ 0.924Q�65   � 5M      $ 	#FF111114��6#   $ 0.436Q�65   � 5N      $ Black��6#   $ 0.192Q�61   � 5O      $ 	#FF1D1D1D��6#   $ 0.628Q�65   5P      5R      �� 5S   
   -   5        $��ProgressLabel��5S   _   
���   �$��28��6#   $
��Left=�6/   $��236,485,0,0q�6J   $	��Top=�6t   $	��506��6�   $��Progresso do download��6�   $��Center=�6�   �� 5T   
   -   5        $��PauseButton��5T   �    .  $��0q�6:   $��Pausar��6o   $��23��6�   $
��Left=�6�   $��257,448,0,0q�6�   $	��Top=�6�   $��75��6�   ��6#   Z� ��6N   �� *PauseCommand+��5U      �� 5V      $ 0.5,1&�6&   $ 0.5,0&�67   .��5W      � $ 	#FF0096FF��6#   $
 0.25Q�65   � 5X      $ 	#FF1569A0��6#   $ 0.814Q�65   5Y      5[      �� 5\   
   -   5        $��border1��5\   y   ���	#FF0D0D0D$��1q�6)   $��84��6=   $
��Left=�6I   $��503,0,0,172q�6d   $��Bottom=�6�   $	��316��6�    �� CornerRadius$ 5��6�   
�������._�5]      �  � RowDefinitions 5^      �� 5_      $	��25* �6$   �� 5`      $	��57* �6$   .+�5b      �� -   5        $��FileDate��5b   �   $�� ��6   $��27��6#   $
��Left=�6/   $��6,6,0,0q�6J   $	��Top=�6[   $	��302��6s   $��12(�6   $��2��6�   �� 5c      -   5        $��FileType��5c   i   $�� ��6   $��12(�6#   $��24��61   $
��Left=�6=   $��6,3,0,0q�6X   $	��Top=�6y   $	��302��6�   $��1��6�   �� 5d      -   5        $��FileSize��5d   N   $�� ��6   $��12(�6#   $��26��61   $��6,0,6,3q�6=   $��Bottom=�6^   $��1��6y   �� 5e      -   5        $��VersionButton��5e   e   $��Versões��6   $��23��6+   $
��Left=�67   $��	228,8,0,0q�6R   $	��Top=�6z   $��75��6�   {�.  J� IsHitTestVisible . $y�Visible=�6�   $��2��6�   ��6�   �� *VersionCommand+6�   �� 5f      -   5        $��
BackButton��5f   �   $��Voltar��6;   $��23��6L   $
��Left=�6X   {�.  . $��
228,28,0,0q�6�   $	��Top=�6�   $y�Visible=�6�   $��75��6�   $��1��6�   ��6   �� *UpdateCommand+6�   5g      5h      �� 5i   
   -   5        $��ResumeButton��5i   �    .  $��0q�6:   $��	Continuar��6p   $��23��6�   $
��Left=�6�   $��443,448,0,0q�6�   $	��Top=�6�   $��75��6�   ��6#   Z� ��6N   �� *ResumeCommand+��5j      �� 5k      $ 0.5,1&�6&   $ 0.5,0&�67   .��5l      � $ 	#FF0096FF��6#   $
 0.25Q�65   � 5m      $ 	#FF1569A0��6#   $ 0.814Q�65   5n      5p      �� 5q   
   -   5        $��ProgressCountLabel��5q   �   
���   �$��28��6#   $
��Left=�6/   $��Center=�6J   $��236,542,0,0q�6n   $	��Top=�6�   $	��506��6�   �� 5r   
   -   5        $��FileList��5r       .  
�������$��504,170,108,261q�6f    �� SelectionMode$ Extended=�6   $��1q�6�   
���   �$ 1Q�6�   |�.  $	��316��6  ��6H   �� *Files+ �� View 5s      �� 5t      .P�5u      ��   �� Width$	  157��6O   $
��Nome��6[   ! �� DisplayMemberBinding! 6%   �� *
FileName+6[   �� 5v      $	  157��6K   $��Tamanho��6W   ! 6%   �� *Size+6W   5w      5y      �� 5z   
   -   5        $��FilesExistLabel��5z   j   $��{FilesExistLabel}��6   $��28��6.   $
��Left=�6:   $��504,200,0,0q�6U   $	��Top=�6�   ���	#FFFC0000$	��316��6�   $��Center=�6�   9� 5|   
   -   5        $��DetailExpander��5|   `   $��Detalhes��6   $	��100��6%   $
��Left=�62   $��	9,495,0,0q�6M   $	��Top=�6v   $	��225��6�   $��Center=�6�   .��5}      �� -   5        $��DownloadDetails��5}       .  
�������$ Single=�6v   $��1q�6�   
���   �$ 1Q�6�   |�.  $	��220��6�   ��6S   �� *
DetailList+ 5~      �� 5      .P�5�      �� $	  110��6S   $��Arquivo��6_   ! 6)   �� *
FileName+6_   �� 5�      $	  110��6Q   $��Status��6]   ! 6)   �� *Status+6]   5�      5�      5�      5�      5�      