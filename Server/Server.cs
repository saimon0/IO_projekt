using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MultiThreadedEchoServer
{
    static int wp = 0; //zmienna zmieniająca wartość jeżeli któryś z graczy wygrał
    static int obliczona; // obliczona średnia wartość nieparzystych liczb 
    static int x = Losowanie();//zmienna przechowująca wylosowaną liczbę
    static DateTime czas = new DateTime(DateTime.Now.Ticks);
    static List<int> intList = new List<int>();// lista przechowująca przesłane liczby nieparzyste
    private static void ProcessClientRequests(object argument)
    {
        DateTimeOffset dto;
        dto = DateTimeOffset.Now;

        char[] buffer = new char[256];// bufor danych
        string theString = null;
        int bytesRead = 0;
        int id = Losowanie();
        DateTime czas = new DateTime(DateTime.Now.Ticks); // teraźniejszy czas
        TcpClient client = (TcpClient)argument;
        try
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream());// utworzenie writer do wysyłania danych
            System.IO.StreamReader reader = new System.IO.StreamReader(client.GetStream());// utworzenie reader do odbierania danych
            string s = String.Empty;


            writer.Write("op#" + "polaczenie/" + "od#polaczono" + "/" + "id#" + id + "/" + "zc#" + dto.ToUnixTimeSeconds() + "/"); // jedne z przesyłanych danych

            writer.Flush();
            while (!s.Equals("Exit")) // główna pętla
            {

                reader.Read(buffer, 0, 256);//odczytywanie przysłanych danych i zapisywanie w buforze

                s = new string(buffer);
                Regex regex_liczbaprob = new Regex("(op#)(liczbaprob)(/od#brak)(/id#)([0-9]{0,5})(/wr#)([0-9]{0,5})");// regex sprawdzający przesyłane dane
                if (regex_liczbaprob.IsMatch(s))
                {
                    Console.WriteLine(x);
                    MatchCollection matchCollection = regex_liczbaprob.Matches(s);
                    foreach (Match match in matchCollection)
                    {

                        intList.Add(System.Convert.ToInt32(match.Groups[7].Value));// dodanie nieparzystej wartości do tablicy
                        id = System.Convert.ToInt32(match.Groups[5].Value);//pobranie id klienta
                    }
                    Console.ReadKey();
                    obliczona = (intList[0] + intList[1]) / 2;// obliczenie średiej wartości przesłanych liczb
                    writer.Write("op#" + "liczbaprob/" + "od#" + obliczona + "/" + "id#" + id + "/" + "zc#" + dto.ToUnixTimeSeconds() + "/");
                    writer.Flush();
                }
                Regex regex_odgadywanie = new Regex("(odgadywanie)/(od#brak)/(id#)([0-9]{0,5})/(wr#)([1-9][0-9]{0,2})");
                if (regex_odgadywanie.IsMatch(s))
                {
                    if (wp == 2)
                    {
                        writer.Write("op#" + "wynik/" + "od#" + "przegrales" + "/" + "id#" + id + "/" + "zc#" + dto.ToUnixTimeSeconds() + "/");
                        Console.WriteLine("zatwierdz");
                        Console.ReadKey();
                    }
                    else
                    {
                        MatchCollection matchCollection = regex_odgadywanie.Matches(s);
                        foreach (Match match in matchCollection)
                        {
                            Console.WriteLine(match.Groups[6].Value);
                            if (System.Convert.ToInt32(match.Groups[6].Value) == x)// sprawdzanie czy wartość jest wartością wylosowaną 
                            {
                                writer.Write("op#" + "wynik/" + "od#" + "wygrales" + "/" + "id#" + id + "/" + "zc#" + dto.ToUnixTimeSeconds() + "/");
                                wp = 1;
                                Console.WriteLine("zatwierdz");
                                Console.ReadKey();
                                break;
                            }
                            else
                            {
                                writer.Write("op#" + "wynik/" + "od#" + "blednaodpowiedz" + "/" + "id#" + id + "/" + "zc#" + dto.ToUnixTimeSeconds() + "/");
                            }
                        }
                    }
                }
                writer.Flush();
            }
            reader.Close();
            writer.Close();
            client.Close();
            Console.WriteLine("Kończenie połączenia z klientem!");
        }
        catch (IOException)
        {
            Console.WriteLine("Połączenie zostało zakończone.");
        }
        finally
        {
            if (client != null)
            {
                client.Close();
            }
        }
    }

    public static void Main()
    {
        TcpListener listener = null;
        try
        {
            Console.WriteLine("Wpisz adres ip:");
            String ip = null;
            ip = Console.ReadLine();
            listener = new TcpListener(IPAddress.Parse(ip), 39999);// ustawienie nasłuchiwania na dany port
            listener.Start();
            Console.WriteLine("Serwer włączony");


            TcpClient client1 = listener.AcceptTcpClient();// zaakceptowanie klienta
            Console.WriteLine("Zaakceptowano nowe połączenie klienta...");
            Thread t1 = new Thread(ProcessClientRequests);
            t1.Start(client1);

            TcpClient client2 = listener.AcceptTcpClient();
            Console.WriteLine("Zaakceptowano nowe połączenie klienta...");
            Thread t2 = new Thread(ProcessClientRequests);
            t2.Start(client2);

            Console.ReadKey();


        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
    static public void Losowanie()
    {
        Console.WriteLine("Tutaj bedzie funkcjonalnosc odpowiedzialna za losowanie liczby");
    }
    public void Obliczanie_Prob(int x1, int x2, StreamWriter writer)
    {
        Console.WriteLine("Tutaj bedzie funkcjonalnosc odpowiedzialna za obliczanie liczby prob do odgadniecia");
    }
    public static void sprawdzanie(int x1, int v1, StreamWriter writer)
    {
        Console.WriteLine("Tutaj bedzie funkcjonalnosc odpowiedzialna za sprawdzenie czy  wartość została odgadnięta ostatecznie nieużywana");
    }

}

