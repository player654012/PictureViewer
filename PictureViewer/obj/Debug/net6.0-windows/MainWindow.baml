   M S B A M L   `   `   `  ����H  DPictureViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:PictureViewerPictureViewer  5   	       PictureViewer.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:PictureViewer     5   	     � Title$  PictureViewer��5   	   $	��450��6   $	��800��6,   
������ .��5
      �� ���#192841 �� CornerRadius$ 10��5      ._�5   
   �  � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��3* �6#    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� $��PictureViewer��6   $��Center=�5      $��Center=�5      
�������$��22(�5      $��
0 10 20 10q�5      �� 5       �� Orientation$ Vertical=�6   $��1��60   $��0��6=   ���#152238.+�5      �� -   5         ��Name$ 
loadButton��5      $��
Load Image��5      $��30 50 30 30q�6.   $��5q�6C   $��16(�5       ���	#FFF0E9E9$��Bahnschrift)�5!      �� 5"      -   5        $ 
saveButton��5"      $��
Save Image��5#      $��30 10q�6.   $��5q�6=   $��Bahnschrift)�5$      $��16(�62   ���	#FFF0E9E95%      5&      �� 5(      -   5         ��Name$ canvas��5(      $��1��6%   $��1��62   $
��Left=�5)      $	��Top=�60   .+�5*      �� -   5         ��Name$ image��5*      $	��300��5-      $	��500��6(   5.      �� 50      $ 
Horizontal=�6   $��1��63   ���#152238.+�52      	� -   5         	�Name$ redRadioButton��52      	 	� 	GroupName$	 Colors��66   
 d� 	IsChecked$

 TrueY�53      $	��Red��54   6   $��50 15 10 15q�6D   
�������$��14(�55   1   	� 56      -   5        $ blackRadioButton��56      $	 Colors��68   $��Black��57   6   $��10 15q�6F   
�������$��14(�58   1   	� 59      -   5        $ blueRadioButton��59      $	 Colors��67   $
��Blue��5:   6   $��10 15q�6E   
�������$��14(�5;   1   	� 5<      -   5        $ greenRadioButton��5<      $	 Colors��68   $��Green��5=   6   $��10 15q�6F   
�������$��14(�5>   1   5?      5@      5A      5B      