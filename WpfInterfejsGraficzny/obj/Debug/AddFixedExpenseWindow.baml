   M S B A M L   `   `   `  ����P  LWpfInterfejsGraficzny, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:WpfInterfejsGraficznyWpfInterfejsGraficzny  5   	   1    +WpfInterfejsGraficzny.AddFixedExpenseWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:WpfInterfejsGraficzny     5   	     � Title$  AddFixedExpenseWindow��5   	   $	��300��6'   $	��300��64   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��60 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      $��25 �6   �� 5      $��25 �6   �� 5      $��25 �6   �� 5      $��25 �6   .+�5   
   �� $��0��6   $��0��6   $��Nazwa��6-   �� 5   
   -   5        $��TB_Name��5   /   $��0��6   $��1��6   6/   �� 5   
   $��1��6   $��0��6   $
��Opis��6-   �� 5   
   -   5        $��TB_Description��5   /   $��1��6   $��1��6   6/   �� 5   
   $��2��6   $��0��6   $��Ilosc��6-   �� 5   
   -   5        $��TB_quantity��5   /   $��2��6   $��1��6   6/   �� 5   
   $��Wydatek��6   $��3��6"   $��0��6/   �� 5    
   -   5        $��
TB_Expense��5    /   $��3��6   $��1��6   6/   � 5"   
   $��4��6   $��0��6   $��2��6,    5#      �� 5$      $��* �6#   �� 5%      $��20 �6#   �� 5&      $��* �6#   .+�5(      �� -   5        $��0��5(   0   $��Dodaj wydatek stały��6@   �� 5)      -   5        $��2��5)   &   $��Zamknij��66   5*      5+      5,      