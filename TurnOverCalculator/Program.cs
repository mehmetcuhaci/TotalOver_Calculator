using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;

namespace TurnOverCalculator
{
    class Program
    {
        static void Main(string[] args)
        {


            TotalTur();

        }


        public static void TotalTur()
        {
            try
            {     // giriş için gerekli url ve şifreleri vs ayarla
                string totalTurnoverUrl = "https://service.foreks.com/feed/snapshot?code=XUTUM&f=Code,Last,DateTime,TotalTurnover";
                string totalTurnoverCurrencyUrl = "https://service.foreks.com/feed/snapshot?domain=VIOP&exchange=BIST&status=ACTIVE&virtual=0&f=Code,TotalTurnoverCurrency";

                string id = "atayatirimfeed";
                string password = "n2&s5a9&";

                // giriş bilgilerini convertle ve işle
                string base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{id}:{password}"));

                using (HttpClient httpClient = new HttpClient())
                {
                    // servise erişim için gerekli headerlar
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Credentials}");
                    httpClient.DefaultRequestHeaders.Add("company", "FOREKS");
                    httpClient.DefaultRequestHeaders.Add("platform", "web");
                    httpClient.DefaultRequestHeaders.Add("app-name", "ata_web");
                    httpClient.DefaultRequestHeaders.Add("app-version", "1.0");

                    // İlk url'ye giriş yap
                    HttpResponseMessage response1 = httpClient.GetAsync(totalTurnoverUrl).Result;
                    response1.EnsureSuccessStatusCode();

                    // Gelen datayı işle
                    string content1 = response1.Content.ReadAsStringAsync().Result;
                    List<SnapshotData> totalTurnoverData = JsonSerializer.Deserialize<List<SnapshotData>>(content1);

                    // gelen data içinde totalTunover'ı işle ve yazdır
                    foreach (var item in totalTurnoverData)
                    {
                        Console.WriteLine($"TotalTurnover: {item.TotalTurnover}");
                    }

                    //İkinci url'ye istek gönder
                    HttpResponseMessage response2 = httpClient.GetAsync(totalTurnoverCurrencyUrl).Result;
                    response2.EnsureSuccessStatusCode();

                    // Gelen datayı işle ve çevir
                    string content2 = response2.Content.ReadAsStringAsync().Result;
                    List<SnapshotDataCurrency> totalTurnoverCurrencyData = JsonSerializer.Deserialize<List<SnapshotDataCurrency>>(content2);

                    //Console.WriteLine("\nTotalTurnoverCurrency data:");
                    // gelen data içinde totalTurnoverCurrencyData'yı işle ve yazdır
                    foreach (var item in totalTurnoverCurrencyData)
                    {
                        // Console.WriteLine($"TotalTurnoverCurrency: {item.TotalTurnoverCurrency}");
                    }

                    // toplamı hesapla ve yazdır
                    double totalCurrencySum = totalTurnoverCurrencyData.Sum(item => item.TotalTurnoverCurrency);
                    Console.WriteLine($"TotalTurnoverCurrency: {totalCurrencySum}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }


            Console.ReadLine();

            
        }
        

        
    }
   


    public class SnapshotData
    {
        public string _id { get; set; }
        public string Code { get; set; }
        public double Last { get; set; }
        public long DateTime { get; set; }
        public double TotalTurnover { get; set; }
    }

    public class SnapshotDataCurrency
    {
        public string _id { get; set; }
        public string Code { get; set; }
        public double TotalTurnoverCurrency { get; set; }
    }



}
