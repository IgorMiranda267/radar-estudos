namespace AIStudyLab.Application.Contracts;

/// <summary>Resumo calculado a partir dos registros de estudo do usuário.</summary>
public sealed record DashboardResponse(
    int TotalDisciplinas,
    int TotalAssuntos,
    int TotalSessoes,
    int Questoes,
    int Acertos,
    int Erros,
    int TempoEstudadoMinutos,
    int RevisoesPendentes,
    IReadOnlyList<ResumoDiarioResponse> UltimosSeteDias);

/// <summary>Resumo de estudo registrado em um dia.</summary>
public sealed record ResumoDiarioResponse(DateOnly Data, int TempoEstudadoMinutos, int Questoes, int Acertos);
