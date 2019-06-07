using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Binance.Net.Objects;
using Domain;
namespace Utils
{
    public static class RequestHelper
    {
        public static String GetHash(String text, String key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        public static Decimal GetOrder(BinanceAggregatedTrades[] recentTrade) {

            decimal BuyOrdersSum = 0;
            decimal SellOrdersSum = 0;

            foreach (var x in recentTrade)
            {
                if (x.BuyerWasMaker)
                {
                    BuyOrdersSum += x.Quantity;
                }
                else
                {
                    SellOrdersSum += x.Quantity;
                }
            }
            var result = BuyOrdersSum - SellOrdersSum;

            return result;
        }
        public static Decimal GetOrderBTC(BinanceAggregatedTrades[] recentTrades) {

            decimal BuyOrdersSum = 0;
            decimal SellOrdersSum = 0;

            foreach (var x in recentTrades)
            {
                if (x.BuyerWasMaker)
                {
                    BuyOrdersSum += (x.Quantity * x.Price);
                }
                else
                {
                    SellOrdersSum += (x.Quantity * x.Price);
                }
            }
            var result = Math.Abs(BuyOrdersSum - SellOrdersSum);

            return result;
        }
        public static Decimal CalculatePercent(BinanceKline[] kline) {

            decimal percent = 0;
            decimal factor;
           
            percent = 100 * (kline[0].Open - kline[0].Close) / kline[0].Open;

            factor = Convert.ToDecimal(Math.Pow(10, 3));
            
            return Math.Round(percent * factor) / factor;
        }

        public static void DoWork(object source)
        {

        }

        public static string[] RealAlgoritm(List<Quotation> Data, List<Quotation> DataRed, decimal inaccuracy, string Pair)
        {
            List<string> kline = new List<string>();
            var response = new List<string>();

            string Kline;

            kline.Clear();
            response.Clear();
            int sumPlus = 0;
            int k = 0;
            int kolvo = 0;


            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Close >= Data[i].Open)
                {
                    Kline = "Green";
                    kline.Add(Kline);
                }
                else
                {
                    Kline = "Red";
                    kline.Add(Kline);
                }
            }
            for (int i = 0; i < Data.Count;)
            {
                try
                {
                    if (k >= kline.Count - 1)
                    {
                        break;
                    }
                    if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Green"))
                    {
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].MaxPrice - Data[i].Close) <= inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy))
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                Data.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    Data.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                Data.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Green"))
                    {
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].Open - Data[i].Close) <= inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice)) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                Data.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    Data.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                Data.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Red"))
                    {
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].MaxPrice - Data[i].Open) <= inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy))
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                Data.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    Data.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                Data.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    else if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Red"))
                    {
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].Close - Data[i].Open) <= inaccuracy) && ((Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice)) <= inaccuracy))
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                Data.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    Data.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                Data.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    if (sumPlus == 3)
                    {
                        try
                        {
                            if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Green") && Data[i].Close > Data[i + 1].Close)
                                || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Green") && Data[i].Close > Data[i + 1].Open))
                            {
                                if (kline[k + 3].StartsWith("Green") && Data[i].Close > Data[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Close > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && Data[i].Close > Data[i + 2].Open)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Close > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                }
                            }
                            else if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Red") && Data[i].Open > Data[i + 1].Close)
                                    || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Red") && Data[i].Open > Data[i + 1].Open))
                            {
                                if (kline[k + 3].StartsWith("Green") && Data[i].Open > Data[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Open > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && Data[i].Open > Data[i + 2].Open)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Open > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + Pair;
                                        response.Add(date);
                                    }
                                }
                            }


                            kolvo++;
                            k++;
                            i = 0;

                            Data.RemoveAt(0);

                            for (int g = 0; g < sumPlus; g++)
                            {
                                Data.RemoveAt(0);
                            }
                            sumPlus = 0;
                        }
                        catch
                        {
                            break;
                        }
                    }
                    k++;
                }
                catch
                {
                    break;
                }
            }
            k = 0;
            for (int i = 0; i < DataRed.Count;)
            {
                try
                {
                    if (k >= kline.Count - 1)
                    {
                        break;
                    }
                    if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Green"))
                    {
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Open) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy))
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                DataRed.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    DataRed.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                DataRed.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Green"))
                    {
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice)) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                DataRed.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    DataRed.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                DataRed.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Red"))
                    {
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy))
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                DataRed.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    DataRed.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                DataRed.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    else if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Red"))
                    {
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice)) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else
                        {
                            if (sumPlus > 0)
                            {
                                DataRed.RemoveAt(0);
                                for (int j = 0; j < sumPlus; j++)
                                {
                                    DataRed.RemoveAt(0);
                                }
                                sumPlus = 0;
                                i = 0;
                            }
                            else
                            {
                                DataRed.RemoveAt(0);
                                sumPlus = 0;
                                i = 0;
                            }
                        }
                    }
                    if (sumPlus == 3)
                    {
                        try
                        {
                            if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Green") && DataRed[i].Open < DataRed[i + 1].Open)
                                || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Green") && DataRed[i].Open < DataRed[i + 1].Close))
                            {
                                if (kline[k + 3].StartsWith("Green") && DataRed[i].Open < DataRed[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && DataRed[i].Open < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && DataRed[i].Open < DataRed[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && DataRed[i].Open < DataRed[i + 3].Open)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                }
                            }
                            else if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Red") && DataRed[i].Close < DataRed[i + 1].Open)
                                    || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Red") && DataRed[i].Close < DataRed[i + 1].Close))
                            {
                                if (kline[k + 3].StartsWith("Green") && DataRed[i].Close < DataRed[i + 2].Open)
                                {
                                    if (kline[k + 4].StartsWith("Green") && DataRed[i].Close < DataRed[i + 3].Open)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && DataRed[i].Close < DataRed[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && DataRed[i].Close < DataRed[i + 3].Open)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + Pair;
                                        response.Add(date);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            break;
                        }

                        kolvo++;
                        k++;
                        i = 0;

                        DataRed.RemoveAt(0);

                        for (int g = 0; g < sumPlus; g++)
                        {
                            DataRed.RemoveAt(0);
                        }

                        sumPlus = 0;
                    }
                    k++;
                }
                catch
                {
                    break;
                }
            }
            return response.ToArray();
        }
    }
}
