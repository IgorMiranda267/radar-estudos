using AIStudyLab.Domain.Entities;

namespace AIStudyLab.Domain.Tests;

public class SessaoEstudoTests
{
    [Fact]
    public void Constructor_AcceptsConsistentQuestionCounts()
    {
        var session = new SessaoEstudo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 11, 9),
            tempoEstudadoMinutos: 90,
            questoes: 10,
            acertos: 7,
            erros: 3);

        Assert.Equal(10, session.Questoes);
        Assert.Equal(7, session.Acertos);
        Assert.Equal(3, session.Erros);
    }

    [Fact]
    public void Constructor_RejectsNegativeStudyTime()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateSession(tempoEstudadoMinutos: -1));
    }

    [Fact]
    public void Constructor_RejectsNegativeQuestionCounts()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateSession(questoes: -1, acertos: 0, erros: 0));
    }

    [Fact]
    public void Constructor_RequiresQuestionsToEqualCorrectPlusWrongAnswers()
    {
        Assert.Throws<ArgumentException>(() => CreateSession(questoes: 10, acertos: 7, erros: 2));
    }

    private static SessaoEstudo CreateSession(
        int tempoEstudadoMinutos = 30,
        int questoes = 10,
        int acertos = 7,
        int erros = 3)
    {
        return new SessaoEstudo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 11, 9),
            tempoEstudadoMinutos,
            questoes,
            acertos,
            erros);
    }
}
