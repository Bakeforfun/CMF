   M S B A M L   `   `   `  ���� H  DOptionPricing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:OptionPricing.ViewModelsOptionPricing.ViewModels  C&clr-namespace:OptionPricing.ConvertersOptionPricing.Converters  &     OptionPricing.Views.PricingShell   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     =
viewModels&clr-namespace:OptionPricing.ViewModels     =
converters&clr-namespace:OptionPricing.Converters     a ]Syncfusion.Shared.Wpf, Version=13.4451.0.58, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.SfGrid.WPF, Version=13.4451.0.58, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89,sf!http://schemas.syncfusion.com/wpf     � Title$  Option Pricing Model��$	��600��$
��1000�����#efefef � DataContext 4   .OptionPricing.ViewModels.PricingShellViewModel   � 	Resources *   $OptionPricing.Converters.InverseBool   InverseBool*   $OptionPricing.Converters.DoubleToPct  DoubleToPct+   %OptionPricing.Converters.TimeSpanDays  TimeSpanDays �� 
TargetType �� Property �� Value  Plus 
 Header-  'Syncfusion.Windows.Shared.MaskedTextBox-  'Syncfusion.Windows.Shared.DoubleTextBox.  (Syncfusion.Windows.Shared.IntegerTextBox,  &Syncfusion.Windows.Shared.DateTimeEdit.	  (Syncfusion.Windows.Shared.PercentTextBox��%%  &	        &	      &	 
     '��      ']� 1     '�� S     &	 z     &	 �     '  �     '      '  .    '  U    '	  |          �� " ��.���  � ��$ 5q��� " ]�.���  � ��$ 5q��� " ��.���  � ��$ Center=��� " ��.���  � ��$ Center=���  � ��$ 18(��� " ��.���  � ��$ DemiBold"��� "  .���  � ��$ Center=��� "  .���  � ��$ Center=��� "  .���  � ��$ Center=��� "  .���  � ��$ Center=��� " 	 .���  � ��$ Center=�.���  � RowDefinitions �� $
��Auto ��� $��* �.+��� $��0��$��0,0,0,1q����#999999���#dddddd�� $��0��.+��  � ColumnDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0�� �� Orientation$ 
Horizontal=�.+��� $��Underlying:  ��  $	��100������ *
Underlying+�� $��1��$ 
Horizontal=�.+��� $��Underlying Price:  ��  $	��100��	   Value	 �� *UnderlyingPrice+
 �� UpdateSourceTrigger$
 PropertyChanged=��� $��2��$ 
Horizontal=�.+��� $��Valuation date:��   J� 	AllowDrop .  ,
  &Syncfusion.Windows.Shared.DateTimeBase 
  IsPopupEnabled .   
  IsButtonPopUpEnabled .  $	��100�� 
  Pattern$ CustomPattern=� 
  CustomPattern$ 
dd-MM-yyyy��   DateTime �� *ValuationDate+�� $��3��$ 
Horizontal=�.+��� $��
Vol Shift:��	   	  PercentValue �� *
VolShift+ �� 	Converter# [ $
 PropertyChanged=��� $��4��$ 
Horizontal=�.+��� $��Rate Shift:��	   �� *	RateShift+# [ $
 PropertyChanged=�� $��1�� �� $��* �$	��200��$	��350���� $��* �$	��200��$	��350���� $��2* �.+�� $��0�� �� $��* ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��2��.+��� $��OPTION PARAMETERS��#��[ �� $��3��$��0��.+��� $��Style:���� $��3��$��1��.+��� ���� *OptionStyles+ �� SelectedItem �� *SelectedStyle+�� $��4��$��0��.+��� $��	Exercise:���� $��4��$��1��.+��� ���� *OptionExerciseTypes+ �� *SelectedExerciseType+�� $��5��$��0��.+��� $��	Call/Put:���� $��5��$��1��.+��� ���� *OptionTypes+ �� *SelectedType+�� $��6��$��0��.+��� $��
Direction:���� $��6��$��1��$ 
Horizontal=�$��Center=�.+�	� $	��Buy�� 	� 	GroupName$ BuySell��$��0,0,10,0q� d� 	IsChecked �� *IsBuy+	� $
��Sell��$ BuySell�� �� *IsBuy+# [  �� $��7��$��0��.+��� $��Strike:���� $��7��$��1��.+�  	 �� *StrikePrice+$
 PropertyChanged=��� $��8��$��0��.+��� $��Strike (%):���� $��8��$��1��.+�	   	  PercentDecimalDigits$ 2��{�.   �� *StrikePricePct+ �� Mode$ OneWay=�# [ �� $��9��$��0��.+��� $��	Quantity:���� $��9��$��1��.+�     Value �� *
Quantity+$
 PropertyChanged=��� $��11��$��0��.+��� $��Expiry:���� $��11��$��1��.+�   .   .   .  $ CustomPattern=�$ 
dd-MM-yyyy�� �� *ExpirationDate+$
 PropertyChanged=��� $��12��$��0��.+��� $��Time to expiry:���� $��12��$��1��$ 
Horizontal=�$��Center=�.+��� ���� *TimeToExpiry+ �� StringFormat$ dd���� $�� days���� $��13��$��0��.+��� $��Model:���� $��13��$��1��.+��� ���� *PricingModels+ �� *SelectedPricingModel+�� $��14��$��0��.+��� $��Volatility:���� $��14��$��1��.+�	  $ 2�� �� *
Volatility+# [ $
 PropertyChanged=��� $��15��$��0��.+��� $��	Int.Rate:���� $��15��$��1��.+�	  $ 2�� �� *InterestRate+# [ $
 PropertyChanged=��� $��16��$��0��.+��� $��	Div.Rate:���� $��16��$��1��.+�	  $ 2�� �� *DividendRate+# [ $
 PropertyChanged=��� $��17��$��0��$��2��$��2��.+��� $��Add to portfolio������ *
AddCommand+� $��1�� �� $��* ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��2��.+��� $��RESULTS OF CALCULATION��#��[ �� $��1��$��0��.+��� $��Option Price:���� $��1��$��1��.+��� ���� *CurrentOption.OptionPrice+$ N2���� $��2��$��0��.+��� $��Percent Price:���� $��2��$��1��.+��� ���� *CurrentOption.PctPrice+$ N2���� $��3��$��0��.+��� $��Total Price:���� $��3��$��1��.+��� ���� *CurrentOption.PositionPrice+$ N2���� $��4��$��0��.+��� $��
Delta (%):���� $��4��$��1��.+��� ���� *CurrentOption.Delta+$ N2���� $��5��$��0��.+��� $��Delta (shares):���� $��5��$��1��.+��� ���� *CurrentOption.PositionDelta+$ N2���� $��6��$��0��.+��� $��Cash Delta:���� $��6��$��1��.+��� ���� *CurrentOption.CashDelta+$ N2���� $��7��$��0��.+��� $��
Gamma ($):���� $��7��$��1��.+��� ���� *CurrentOption.CashGamma+$ N2���� $��8��$��0��.+��� $��	Vega ($):���� $��8��$��1��.+��� ���� *CurrentOption.CashVega+$ N2���� $��9��$��0��.+��� $��
Theta ($):���� $��9��$��1��.+��� ���� *CurrentOption.CashTheta+$ N2���� $��10��$��0��.+��� $��Rho ($):���� $��10��$��1��.+��� ���� *CurrentOption.CashRho+$ N2��� $��2�� �� $
��Auto ��� $
��Auto ��� $��* �.+��� $��1��$ 
Horizontal=�.+��� $��OPTIONS PORTFOLIO    ��#��[ ]� $��2��$��0��.+�(  "Syncfusion.UI.Xaml.Grid.SfDataGrid     AutoGenerateColumns .     ColumnSizer$ SizeToCells=�   AllowDeleting .    AllowResizingColumns .    ItemsSource �� *OptionsPortfolio+   Columns /  )Syncfusion.UI.Xaml.Grid.GridNumericColumn  (  "Syncfusion.UI.Xaml.Grid.GridColumn    AllowResizing  .  !   ColumnSizer$! SizeToHeader=�"   
HeaderText$" #��#   NumberDecimalDigits$# 0��$   ValueBinding$ �� *Id+,  &Syncfusion.UI.Xaml.Grid.GridTextColumn    .  $! SizeToHeader=�$
" Type��$ �� *Type+    .  $! SizeToHeader=�$
" Side��$ �� *Side+  $! SizeToHeader=�$
" Qnty��$# 0��$ �� *
Quantity+  $! SizeToHeader=�$" Strike��$# 2��$ �� *Strike+  $! SizeToHeader=�$
" Time��$# 0��$ �� *TimeToExpiry+# [   $! SizeToHeader=�%   MaximumWidth$	% 200Q�$" Value��$# 2��$ �� *PositionPrice+  $! SizeToHeader=�$	% 200Q�$" 
Cost price��$# 2��$ �� *	CostPrice+  $! SizeToHeader=�$	% 200Q�$" PL��$# 2��$ �� *PL+  $! SizeToHeader=�$	% 200Q�$" 	Delta ($)��$# 2��$ �� *	CashDelta+  $! SizeToHeader=�$	% 200Q�$" 	Gamma ($)��$# 2��$ �� *	CashGamma+  $! SizeToHeader=�$	% 200Q�$" Vega ($)��$# 2��$ �� *
CashVega+  $! SizeToHeader=�$	% 200Q�$" 	Theta ($)��$# 2��$ �� *	CashTheta+  $! SizeToHeader=�$	% 200Q�$" Rho($)��$# 2��$ �� *	CashRho+/  )Syncfusion.UI.Xaml.Grid.GridPercentColumn  $! SizeToHeader=�$	% 200Q�$" 
Volatility��&   PercentDecimalDigits$& 2��$ �� *
Volatility+# [   $! SizeToHeader=�$	% 200Q�$
" Rate��$& 2��$ �� *InterestRate+# [ �� $��0��$��0�����#ddddbb$��1,0,0,1q����#999999� $��0��$��0�� �� $	��150���� $	��150���� $	��150���� $	��150�� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��.+��� $��P&L : ��#��[ �� $��0��$��1��.+��� #��[ ���� *PL+$ N2���� $��2��$��0��.+��� $��Delta : ��#��[ �� $��2��$��1��.+��� #��[ ���� *PortfolioDelta+$ N2���� $��2��$��2��.+��� $��Cash Delta : ��#��[ �� $��2��$��3��.+��� #��[ ���� *PortfolioCashDelta+$ N2���� $��3��$��0��.+��� $��Cash Gamma : ��#��[ �� $��3��$��1��.+��� #��[ ���� *PortfolioCashGamma+$ N2���� $��3��$��2��.+��� $��Cash Vega : ��#��[ �� $��3��$��3��.+��� #��[ ���� *PortfolioCashVega+$ N2���� $��4��$��0��.+��� $��Cash Theta : ��#��[ �� $��4��$��1��.+��� #��[ ���� *PortfolioCashTheta+$ N2���� $��4��$��2��.+��� $��Cash Rho : ��#��[ �� $��4��$��3��.+��� #��[ ���� *PortfolioCashRho+$ N2��