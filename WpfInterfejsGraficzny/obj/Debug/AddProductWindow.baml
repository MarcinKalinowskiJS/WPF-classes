   M S B A M L   `   `   `  ����P  LWpfInterfejsGraficzny, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:WpfInterfejsGraficznyWpfInterfejsGraficzny  5   	   ,    &WpfInterfejsGraficzny.AddProductWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:WpfInterfejsGraficzny     5   	     � Title$  AddProductWindow��5   	   $	��300��6"   $	��320��6/    � 	Resources 5	      5   /WpfInterfejsGraficzny.StringCollectionConverter   StringCollectionConverter��%   &	          5   
   6*   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��CB_AvailableProducts��5   |   $��0��6K   $��0��6X   $��2��6h   6|   �� 5   
   $��1��6   $��0��6   $��Nazwa��6-   �� 5   
   -   5        $��TB_Nazwa��5   /   $��1��6   $��1��6   $��Stretch=�6?   �� 5    
   $��2��6   $��0��6   $
��Opis��6-   �� 5!   
   -   5        $��TB_Opis��5!   /   $��2��6   $��1��6   $��Stretch=�6>   �� 5#   
   $��3��6   $��0��6   $��	Kategorie��6-   �� 5$   
   -   5        $��TB_Kategorie��5$   �   $��3��6l   $��1��6y   $��Stretch=�6�   ��6   ��  �� Path$ KategorieBind� �� 	Converter# [  6�   �� 5&   
   $��4��6   $��0��6   $��Ilosc��6-   �� 5'   
   -   5        $��TB_Ilosc��5'   /   $��4��6   $��1��6   $��Stretch=�6?   �� 5)   
   $��5��6   $��0��6   $
��Cena��6-   �� 5*   
   -   5        $��TB_Cena��5*   /   $��5��6   $��1��6   $��Stretch=�6>   �� 5,   
   -   5        $��6��5,   *   $��0��67   $��2��6G   $��Dodaj produkt��6[   5-      5.      