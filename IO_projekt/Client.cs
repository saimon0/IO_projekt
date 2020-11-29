using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
public class EchoClient
{
    static int pw = 0;
    static int id;//zmienna przechowująca id przesłane od serwera
    static int liczba_prob = 0;
    public static void Main()
    {

        try
        {
            DateTimeOffset dto;

            dto = DateTimeOffset.Now;
            char[] buffer = new char[256];//utworzenie bufora
            char[] bufffer = new char[256];
            Console.WriteLine("Wpisz adres ip:");
            String ip = null;
            ip = Console.ReadLine();//wpisywanie adresu ip
            TcpClient client = new TcpClient(ip, 39999);// utworzenie gniazda klienta
            System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream());//utworzenie writer do wysyłania danych
            System.IO.StreamReader reader = new System.IO.StreamReader(client.GetStream());//utworzenie reader do odbierania danych
            String s = String.Empty;
            string p = String.Empty;
            while (!s.Equals("Exit"))//pętla głowna
            {
                reader.Read(buffer, 0, 256);//odczytywanie danych do bufora
                p = new string(buffer);
                Regex identyfikacja = new Regex("(polaczenie)/(od#)(polaczono)/(id#)([0-9]{0,2})");//regex sprawdzający przydzielone id
                MatchCollection zbior = identyfikacja.Matches(p);
                foreach (Match match in zbior)
                {
                    Console.WriteLine(System.Convert.ToString(match.Groups[5].Value));
                    id = System.Convert.ToInt32(match.Groups[5].Value);//zapisanie swojego id sesji
                }
                Console.Write("Wybierz numer zadania:");
                Console.Write("Wybierz 1 aby Wysłać nieparzystą wartość L");
                Console.Write("Wybierz 2 aby Wysłać nieparzystą wartość L");
                s = Console.ReadLine();
                if (s == "1")
                {
                    nieparzysta(writer);// wywołanie metody nieparzysta
                    writer.Flush();
                    reader.Read(bufffer, 0, 256);//odczytanie danych do bufora

                    p = new string(bufffer);

                }
                Regex regex = new Regex("(liczbaprob)/(od#)([1-9][0-9]{0,1})/(id#)([1-9][0-9]{0,1})");
                MatchCollection matchCollection = regex.Matches(p);
                foreach (Match match in matchCollection)
                {
                    Console.WriteLine(match.Groups[3].Value);
                    liczba_prob = System.Convert.ToInt32(match.Groups[3].Value);// zapisanie możliwej liczby prób

                    Console.Write("Obliczona liczba prób to:");
                    Console.WriteLine(match.Groups[3].Value);//wypisanie liczby prób;

                }

                Regex wygrana = new Regex("(wynik)/(od#)(wygrales)/(id#)");//regex wygrana
                Regex przegrana = new Regex("(wynik)/(od#)(przegrales)/(id#)");//regex przegrana
                Regex bledna = new Regex("(op#)(wynik)(/od#blednaodpowiedz)(/id#)([0-9]{0,5})");//regex błedna
                Console.Write("2.Odgadnięcie wylosowanej liczby");
                s = Console.ReadLine();
                if (s == "2")
                {
                    while (!s.Equals("Exit"))
                    {
                        {


                            if (liczba_prob == 0)//sprawdzanie czy została przekroczona liczba prób
                            {
                                Console.WriteLine("Koniec prob, przegrales!");
                            }
                            else
                            {
                                Console.WriteLine("Pozostalo prob:" + liczba_prob);//informowanie ile prób pozostało
                            }
                            liczba_prob -= 1;//dekrementowanie liczby prób

                            odgadywanie(writer);//wywołanie funkcji odgadywanie
                        }
                        reader.Read(buffer, 0, 256);

                        String server_string = new string(buffer);
                        matchCollection = wygrana.Matches(server_string);

                        if (przegrana.IsMatch(server_string))
                        {
                            Console.WriteLine("Przegrałeś");//informowanie o przegranej w konsoli
                            break;
                        }
                        if (wygrana.IsMatch(server_string))
                        {
                            Console.WriteLine("Wygrałeś");//informowanie o wygranej w konsoli
                            pw = 1;

                            break;
                        }

                    }
                    break;

                }
            }
            Console.WriteLine("Wciśnij enter aby zakończyć połączenie.");
            Console.ReadKey();
            reader.Close();//zamykanie odczytującego
            writer.Close();//zamykani wysyłającego
            client.Close();//zamykanie gniazda klienta
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public static void nieparzysta(StreamWriter writer)//metoda nieparzysta
    {
        DateTimeOffset dto;

        dto = DateTimeOffset.Now;
        int liczba;
        int x = 0;
        while (x == 0)
        {
            Console.Write("Podaj liczbę podjęcia prób: ");
            liczba = Convert.ToInt32(Console.ReadLine());
            if (liczba % 2 != 0)//sprawdzanie czy liczba jest nieparzysta
            {
                DateTime czas = new DateTime(DateTime.Now.Ticks);
                writer.Write("op#" + "liczbaprob/" + "od#" + "brak" + "/" + "id#" + id + "/" + "wr#" + liczba + "/" + "zc#" + dto.ToUnixTimeSeconds() + "/");//wysyłanie wartości nieparzystej
                writer.Flush();
                break;
            }
            else
            {
                Console.Write("Wartość parzysta, proszę podać wartość nieparzystą: ");
            }
        }
    }

    public static void odgadywanie(StreamWriter writer)// metoda odpowiedzialna za odgadywanie
    {
        DateTimeOffset dto;

        dto = DateTimeOffset.Now;
        DateTime czas = new DateTime(DateTime.Now.Ticks);

        Console.Write("Podaj odpowiedź: ");
        int liczba = Convert.ToInt32(Console.ReadLine());//przekonwertowana odgadywana liczba
        writer.Write("op#" + "odgadywanie/" + "od#" + "brak" + "/" + "id#" + id + "/" + "wr#" + liczba + "/" + "zc#" + dto.ToUnixTimeSeconds() + "/");//wysyłanie odgadywanej liczby
        writer.Flush();

    }


}