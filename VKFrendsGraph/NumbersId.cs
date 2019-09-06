namespace VKFrendsGraph
{
    public class NumbersId
    {
        public string Id { get; set; } // Id друга
        public int Deep { get; set; } // Глибина пошуку
        public bool Red { get; set; } // Прапор (знайдені друзі?)
        public string Name { get; set; } // Ім'я
        public string Surname { get; set; } // Призвище
    }
}
