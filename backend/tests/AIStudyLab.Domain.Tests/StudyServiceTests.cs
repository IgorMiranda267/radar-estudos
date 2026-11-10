using AIStudyLab.Application.Contracts;
using AIStudyLab.Domain.Entities;
using AIStudyLab.Infrastructure.Persistence;
using AIStudyLab.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIStudyLab.Domain.Tests;

public class StudyServiceTests
{
    [Fact]
    public async Task CriarUsuarioAsync_StoresPasswordHashAndReturnsPublicData()
    {
        await using var context = CreateContext();
        var passwordHasher = new PasswordHasher<Usuario>();
        var service = new StudyService(context, passwordHasher);

        var response = await service.CriarUsuarioAsync(
            new CriarUsuarioRequest
            {
                Nome = "Estudante",
                Email = " ESTUDANTE@example.com ",
                Senha = "SenhaSegura123!"
            },
            CancellationToken.None);

        var storedUser = await context.Usuarios.SingleAsync();
        Assert.Equal("estudante@example.com", response.Email);
        Assert.Equal(PasswordVerificationResult.Success, passwordHasher.VerifyHashedPassword(
            storedUser,
            storedUser.SenhaHash,
            "SenhaSegura123!"));
        Assert.NotEqual("SenhaSegura123!", storedUser.SenhaHash);
    }

    [Fact]
    public async Task CriarSessaoAsync_RejectsSubjectOwnedByAnotherUser()
    {
        await using var context = CreateContext();
        var owner = new Usuario("Dono", "dono@example.com", "hash");
        var otherUser = new Usuario("Outro", "outro@example.com", "hash");
        context.Usuarios.AddRange(owner, otherUser);

        var ownerDiscipline = new Disciplina(owner.Id, "Matemática", "#2563EB", "book");
        var otherDiscipline = new Disciplina(otherUser.Id, "História", "#DC2626", "clock");
        context.Disciplinas.AddRange(ownerDiscipline, otherDiscipline);
        var otherSubject = new Assunto(otherDiscipline.Id, "Brasil República");
        context.Assuntos.Add(otherSubject);
        await context.SaveChangesAsync();

        var service = new StudyService(context, new PasswordHasher<Usuario>());
        var request = new CriarSessaoEstudoRequest
        {
            Data = new DateOnly(2026, 11, 9),
            TempoEstudadoMinutos = 45,
            Questoes = 4,
            Acertos = 3,
            Erros = 1,
            AssuntoId = otherSubject.Id
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.CriarSessaoAsync(owner.Id, ownerDiscipline.Id, request, CancellationToken.None));
    }

    [Fact]
    public async Task ExcluirAssuntoAsync_RejectsSubjectWithStudyHistory()
    {
        await using var context = CreateContext();
        var user = new Usuario("Estudante", "estudante@example.com", "hash");
        var discipline = new Disciplina(user.Id, "Matemática", "#2563EB", "book");
        var subject = new Assunto(discipline.Id, "Funções");
        context.AddRange(user, discipline, subject);
        context.SessoesEstudo.Add(new SessaoEstudo(
            user.Id,
            discipline.Id,
            new DateOnly(2026, 11, 9),
            tempoEstudadoMinutos: 30,
            questoes: 2,
            acertos: 1,
            erros: 1,
            assuntoId: subject.Id));
        await context.SaveChangesAsync();

        var service = new StudyService(context, new PasswordHasher<Usuario>());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ExcluirAssuntoAsync(user.Id, discipline.Id, subject.Id, CancellationToken.None));
    }

    private static StudyLabDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StudyLabDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new StudyLabDbContext(options);
    }
}
