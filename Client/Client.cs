using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

public class EchoClient
{
    static int pw = 0;
    static int id;
    static int liczba_prob = 0;
    public static void Main()
    {

        try
        {
            DateTimeOffset dto;

            dto = DateTimeOffset.Now;
            char[] buffer = new char[256];
            char[] bufffer = new char[256];
            Console.WriteLine("Wpisz adres ip:");
            String ip = null;
            ip = Console.ReadLine();
            TcpClient client = new TcpClient(ip, 39999);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream());
            System.IO.StreamReader reader = new System.IO.StreamReader(client.GetStream());
            String s = String.Empty;
            string p = String.Empty;
            while (!s.Equals("Exit"))
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
                    nieparzysta(writer);
                    writer.Flush();
                    reader.Read(bufffer, 0, 256);

                    p = new string(bufffer);

                }
                Regex regex = new Regex("(liczbaprob)/(od#)([1-9][0-9]{0,1})/(id#)([1-9][0-9]{0,1})");
                MatchCollection matchCollection = regex.Matches(p);
                foreach (Match match in matchCollection)
                {
                    Console.WriteLine(match.Groups[3].Value);
                    liczba_prob = System.Convert.ToInt32(match.Groups[3].Value);

                    Console.Write("Obliczona liczba prób to:");
                    Console.WriteLine(match.Groups[3].Value);

                }

                Regex wygrana = new Regex("(wynik)/(od#)(wygrales)/(id#)");
                Regex przegrana = new Regex("(wynik)/(od#)(przegrales)/(id#)");
                Regex bledna = new Regex("(op#)(wynik)(/od#blednaodpowiedz)(/id#)([0-9]{0,5})");
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
    public static void nieparzysta(StreamWriter writer)
    {
        
    }

    public static void odgadywanie(StreamWriter writer)
    {
        
    }


}