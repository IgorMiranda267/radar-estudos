using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIStudyLab.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senha_hash = table.Column<string>(type: "text", nullable: false),
                    avatar_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ultimo_login_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "disciplinas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cor = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    icone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disciplinas", x => x.id);
                    table.UniqueConstraint("AK_disciplinas_id_usuario_id", x => new { x.id, x.usuario_id });
                    table.ForeignKey(
                        name: "FK_disciplinas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    unidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metas", x => x.id);
                    table.CheckConstraint("ck_metas_intervalo_datas", "data_fim >= data_inicio");
                    table.CheckConstraint("ck_metas_quantidade_positiva", "quantidade > 0");
                    table.ForeignKey(
                        name: "FK_metas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "perfis_usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    objetivo = table.Column<string>(type: "text", nullable: true),
                    nivel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    instituicao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    data_nascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis_usuario", x => x.id);
                    table.ForeignKey(
                        name: "FK_perfis_usuario_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assuntos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    disciplina_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    dificuldade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assuntos", x => x.id);
                    table.UniqueConstraint("AK_assuntos_id_disciplina_id", x => new { x.id, x.disciplina_id });
                    table.CheckConstraint("ck_assuntos_ordem", "ordem >= 1");
                    table.ForeignKey(
                        name: "FK_assuntos_disciplinas_disciplina_id",
                        column: x => x.disciplina_id,
                        principalTable: "disciplinas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "configuracoes_usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tema = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    idioma = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notificacoes_ativadas = table.Column<bool>(type: "boolean", nullable: false),
                    objetivo_diario_minutos = table.Column<int>(type: "integer", nullable: false),
                    disciplina_foco_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracoes_usuario", x => x.id);
                    table.CheckConstraint("ck_configuracoes_usuario_objetivo_diario", "objetivo_diario_minutos >= 0");
                    table.ForeignKey(
                        name: "FK_configuracoes_usuario_disciplinas_disciplina_foco_id_usuari~",
                        columns: x => new { x.disciplina_foco_id, x.usuario_id },
                        principalTable: "disciplinas",
                        principalColumns: new[] { "id", "usuario_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_usuario_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "revisoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assunto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_agendada = table.Column<DateOnly>(type: "date", nullable: false),
                    data_conclusao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revisoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_revisoes_assuntos_assunto_id",
                        column: x => x.assunto_id,
                        principalTable: "assuntos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_revisoes_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sessoes_estudo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    disciplina_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assunto_id = table.Column<Guid>(type: "uuid", nullable: true),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    tempo_estudado_minutos = table.Column<int>(type: "integer", nullable: false),
                    questoes = table.Column<int>(type: "integer", nullable: false),
                    acertos = table.Column<int>(type: "integer", nullable: false),
                    erros = table.Column<int>(type: "integer", nullable: false),
                    banca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessoes_estudo", x => x.id);
                    table.CheckConstraint("ck_sessoes_estudo_questoes_nao_negativas", "questoes >= 0 AND acertos >= 0 AND erros >= 0");
                    table.CheckConstraint("ck_sessoes_estudo_tempo_nao_negativo", "tempo_estudado_minutos >= 0");
                    table.CheckConstraint("ck_sessoes_estudo_total_questoes", "acertos + erros = questoes");
                    table.ForeignKey(
                        name: "FK_sessoes_estudo_assuntos_assunto_id_disciplina_id",
                        columns: x => new { x.assunto_id, x.disciplina_id },
                        principalTable: "assuntos",
                        principalColumns: new[] { "id", "disciplina_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sessoes_estudo_disciplinas_disciplina_id_usuario_id",
                        columns: x => new { x.disciplina_id, x.usuario_id },
                        principalTable: "disciplinas",
                        principalColumns: new[] { "id", "usuario_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sessoes_estudo_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assuntos_disciplina_nome",
                table: "assuntos",
                columns: new[] { "disciplina_id", "nome" });

            migrationBuilder.CreateIndex(
                name: "IX_configuracoes_usuario_disciplina_foco_id_usuario_id",
                table: "configuracoes_usuario",
                columns: new[] { "disciplina_foco_id", "usuario_id" });

            migrationBuilder.CreateIndex(
                name: "ux_configuracoes_usuario_usuario_id",
                table: "configuracoes_usuario",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_disciplinas_usuario_nome",
                table: "disciplinas",
                columns: new[] { "usuario_id", "nome" });

            migrationBuilder.CreateIndex(
                name: "ix_metas_usuario_ativa_inicio",
                table: "metas",
                columns: new[] { "usuario_id", "ativa", "data_inicio" });

            migrationBuilder.CreateIndex(
                name: "ux_perfis_usuario_usuario_id",
                table: "perfis_usuario",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_revisoes_assunto_id",
                table: "revisoes",
                column: "assunto_id");

            migrationBuilder.CreateIndex(
                name: "ix_revisoes_usuario_data_status",
                table: "revisoes",
                columns: new[] { "usuario_id", "data_agendada", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_sessoes_estudo_assunto_id_disciplina_id",
                table: "sessoes_estudo",
                columns: new[] { "assunto_id", "disciplina_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sessoes_estudo_disciplina_data",
                table: "sessoes_estudo",
                columns: new[] { "disciplina_id", "data" });

            migrationBuilder.CreateIndex(
                name: "IX_sessoes_estudo_disciplina_id_usuario_id",
                table: "sessoes_estudo",
                columns: new[] { "disciplina_id", "usuario_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sessoes_estudo_usuario_data",
                table: "sessoes_estudo",
                columns: new[] { "usuario_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracoes_usuario");

            migrationBuilder.DropTable(
                name: "metas");

            migrationBuilder.DropTable(
                name: "perfis_usuario");

            migrationBuilder.DropTable(
                name: "revisoes");

            migrationBuilder.DropTable(
                name: "sessoes_estudo");

            migrationBuilder.DropTable(
                name: "assuntos");

            migrationBuilder.DropTable(
                name: "disciplinas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
