namespace skbidihra;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Pro pohyb pouzivejte w,a,s,d \nPro utok musite byt v blizkosti zloducha a stisknout mezernik ");
        Console.WriteLine("zvolte velikost hraciho pole");
        try
        {
            int[] input = Console.ReadLine().Split(" ").Select(int.Parse).ToArray();
            Hra hra = new Hra(input[0], input[1]);
            hra.Hraj();
        }
        catch (Exception e)
        {
            Console.WriteLine("Spatne zadana velikost pole");
            Environment.Exit(-1);
        }

        Console.ReadLine();
    }
}
