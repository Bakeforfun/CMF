   M S B A M L   `   `   `  ����H  DOptionPricing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:OptionPricing.ViewsOptionPricing.Views  5   	   C&clr-namespace:OptionPricing.ViewModelsOptionPricing.ViewModels  5   	   C&clr-namespace:OptionPricing.ConvertersOptionPricing.Converters  5	   	   &     OptionPricing.Views.PricingShell   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:OptionPricing.Views     5   	   =
viewModels&clr-namespace:OptionPricing.ViewModels     5   	   =
converters&clr-namespace:OptionPricing.Converters     5	   	     � Title$  Option Pricing Model��5   	   $	��600��6&   $	��800��63    � DataContext 5      4   .OptionPricing.ViewModels.PricingShellViewModel  5   
    � 	Resources 5      *   $OptionPricing.Converters.InverseBool   InverseBool �� 
TargetType �� Property �� Value 
 Header��%  &	        '��      ']� Z     &	 �       5   
   6!   �� 5   
   " ��6   .�5      ��  � ��$ 5q�6'   5      �� 5   
   " ]�6   .�5      ��  � ��$ 5q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ DemiBold"�6+   5      5      .��5       �  � ColumnDefinitions 5!   
   �� 5"      $��* �6   $	��200��6)   $	��300��68   �� 5#      $��* �6   $	��200��6)   $	��300��68   �� 5$      $��2* �6   .+�5&   
   � $��0��6    5'      �� 5(      $��* �6#   �� 5)      $��* �6#    � RowDefinitions 5+      �� 5,      $
��Auto �6    �� 5-      $
��Auto �6    �� 5.      $
��Auto �6    �� 5/      $
��Auto �6    �� 50      $
��Auto �6    �� 51      $
��Auto �6    �� 52      $
��Auto �6    �� 53      $
��Auto �6    �� 54      $
��Auto �6    �� 55      $
��Auto �6    �� 56      $
��Auto �6    �� 57      $
��Auto �6    �� 58      $
��Auto �6    �� 59      $
��Auto �6    �� 5:      $
��Auto �6    �� 5;      $
��Auto �6    �� 5<      $
��Auto �6    �� 5=      $
��Auto �6    .+�5?      �� $��0��6   $��0��6&   $��2��66   .+�5@      �� $��OPTION PARAMETERS��6   #��[ 65   5A      �� 5B      $��1��6   $��0��6&   .+�5C      �� $��Underlying:��6   5D      �� 5E      $��1��6   $��1��6&   .+�5F      �� ��6   �� *
Underlying+5G      �� 5H      $��2��6   $��0��6&   .+�5I      �� $��Underlying Price:��6   5J      �� 5K      $��2��6   $��1��6&   .+�5L      �� ��6   �� *UnderlyingPrice+5M      �� 5N      $��3��6   $��0��6&   .+�5O      �� $��Style:��6   5P      �� 5Q      $��3��6   $��1��6&   .+�5R      �� ��6   �� *OptionStyles+ �� SelectedItem 6@   �� *SelectedStyle+5S      �� 5T      $��4��6   $��0��6&   .+�5U      �� $��	Exercise:��6   5V      �� 5W      $��4��6   $��1��6&   .+�5X      �� ��6   �� *OptionExerciseTypes+ 6G   �� *SelectedExerciseType+5Y      �� 5Z      $��5��6   $��0��6&   .+�5[      �� $��	Call/Put:��6   5\      �� 5]      $��5��6   $��1��6&   .+�5^      �� ��6   �� *OptionTypes+ 6?   �� *SelectedType+5_      �� 5`      $��6��6   $��0��6&   .+�5a      �� $��
Direction:��6   5b      �� 5c      $��6��6   $��1��6&   	 �� Orientation$	 
Horizontal=�66   $��Center=�6O   .+�5d      	� $	��Buy��6:   
 	� 	GroupName$
 BuySell��6H   $��0,0,10,0q�6\    d� 	IsChecked 6   �� *IsBuy+6\   	� 5e      $
��Sell��6b   $
 BuySell��5f       5e      �� *IsBuy+ �� 	Converter# [  5f      5g      �� 5h      $��7��6   $��0��6&   .+�5i      �� $��Strike:��6   5j      �� 5k      $��7��6   $��1��6&   .+�5l      �� ��6   �� *StrikePrice+5m      �� 5n      $��8��6   $��0��6&   .+�5o      �� $��Strike (%):��6   5p      �� 5q      $��8��6   $��1��6&   .+�5r      �� ��6   �� *StrikePricePct+5s      �� 5t      $��9��6   $��0��6&   .+�5u      �� $��	Quantity:��6   5v      �� 5w      $��9��6   $��1��6&   .+�5x      �� ��6   �� *
Quantity+5y      �� 5z      $��10��6   $��0��6'   .+�5{      �� $��Valuation date:��6   5|      �� 5}      $��10��6   $��1��6'   .+�5~      (  "System.Windows.Controls.DatePicker     SelectedDate 6   �� *ValuationDate+5      �� 5�      $��11��6   $��0��6'   .+�5�      �� $��Expiry:��6   5�      �� 5�      $��11��6   $��1��6'   .+�5�         6   �� *ExpirationDate+5�      �� 5�      $��12��6   $��0��6'   .+�5�      �� $��Time to expiry:��6   5�      �� 5�      $��12��6   $��1��6'   .+�5�      �� ��6   �� *TimeToExpiry+5�      �� 5�      $��13��6   $��0��6'   .+�5�      �� $��Model:��6   5�      �� 5�      $��13��6   $��1��6'   .+�5�      �� ��6   �� *PricingModels+ 6A   �� *SelectedPricingModel+5�      �� 5�      $��14��6   $��0��6'   .+�5�      �� $��Volatility:��6   5�      �� 5�      $��14��6   $��1��6'   .+�5�      �� ��6   �� *
Volatility+5�      �� 5�      $��15��6   $��0��6'   .+�5�      �� $��	Int.Rate:��6   5�      �� 5�      $��15��6   $��1��6'   .+�5�      �� ��6   �� *InterestRate+5�      �� 5�      $��16��6   $��0��6'   $��2��67   .+�5�      �� $��Compute��6   ��6+   �� *ComputeCommand+5�      �� 5�      $��17��6   $��0��6'   $��2��67   .+�5�      �� $��Add to portfolio��6   ��64   �� *
AddCommand+5�      5�      � 5�   
   $��1��6    5�      �� 5�      $
��Auto �6    �� 5�      $��* �6    .+�5�      �� $��0��6   .+�5�      �� $��COMPUTED PARAMETERS��6   #��[ 67   5�      ]� 5�      $��1��6   $��0��6%   .+�5�      &   System.Windows.Controls.DataGrid  ��6   �� *	Results+5�      5�      � 5�   
   $��2��6    5�      �� 5�      $
��Auto �6    �� 5�      $��2* �6    �� 5�      $��* �6    .+�5�      �� $��0��6   .+�5�      �� $��OPTIONS PORTFOLIO��6   #��[ 65   5�      ]� 5�      $��1��6   $��0��6%   .+�5�           CanUserDeleteRows . ��6   �� *OptionsPortfolio+6D   5�      � 5�      $��2��6   $��0��6     5�      �� 5�      $	��150��6'   �� 5�      $	��150��6'   �� 5�      $	��150��6'   �� 5�      $	��150��6'    5�      �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   .+�5�      �� $��0��6   $��0��6*   $��4��6:   .+�5�      �� $��PORTFOLIO GREEKS��6    #��[ 68   5�      �� 5�      $��1��6   $��0��6*   .+�5�      �� $��Cash Delta : ��6    5�      �� 5�      $��1��6   $��1��6*   .+�5�      �� ��6    �� *PortfolioCashDelta+5�      �� 5�      $��1��6   $��2��6*   .+�5�      �� $��Cash Gamma : ��6    5�      �� 5�      $��1��6   $��3��6*   .+�5�      �� ��6    �� *PortfolioCashGamma+5�      �� 5�      $��2��6   $��0��6*   .+�5�      �� $��Cash Vega : ��6    5�      �� 5�      $��2��6   $��1��6*   .+�5�      �� ��6    �� *PortfolioCashVega+5�      �� 5�      $��2��6   $��2��6*   .+�5�      �� $��Cash Theta : ��6    5�      �� 5�      $��2��6   $��3��6*   .+�5�      �� ��6    �� *PortfolioCashTheta+5�      �� 5�      $��3��6   $��0��6*   .+�5�      �� $��Cash Rho : ��6    5�      �� 5�      $��3��6   $��1��6*   .+�5�      �� ��6    �� *PortfolioCashRho+5�      5�      5�      5�      5�      