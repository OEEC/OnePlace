using System.ComponentModel;

namespace OnePlace.Shared.Enums
{
    public enum TipoQuiz
    {
        [Description("Quiz")]
        Quiz,
        [Description("Recomendación")]
        Recomendacion,
        [Description("Quiz de trato")]
        QuizTrato
    }
    public enum TipoQuizEmpleado
    {
        [Description("Recomendación")]
        Recomendacion,
        [Description("Quiz de trato")]
        QuizTrato
    }
}
