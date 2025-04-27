using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Payments.API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plano",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plano", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "assinatura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdExterno = table.Column<string>(type: "text", nullable: true),
                    Vencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    PlanoId = table.Column<int>(type: "integer", nullable: false),
                    AlunoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    NumeroCartao = table.Column<string>(type: "text", nullable: true),
                    TipoPagamento = table.Column<int>(type: "integer", nullable: false),
                    CopiaCola = table.Column<string>(type: "text", nullable: true),
                    Imagem = table.Column<string>(type: "text", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assinatura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assinatura_plano_PlanoId",
                        column: x => x.PlanoId,
                        principalTable: "plano",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StatusPagamento = table.Column<int>(type: "integer", nullable: false),
                    AssinaturaId = table.Column<int>(type: "integer", nullable: false),
                    PagamentoIdExternal = table.Column<string>(type: "text", nullable: true),
                    AssinaturaExternal = table.Column<string>(type: "text", nullable: true),
                    TipoFaturamento = table.Column<string>(type: "text", nullable: true),
                    FaturaURL = table.Column<string>(type: "text", nullable: true),
                    NumeroFatura = table.Column<string>(type: "text", nullable: true),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagamento_assinatura_AssinaturaId",
                        column: x => x.AssinaturaId,
                        principalTable: "assinatura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assinatura_PlanoId",
                table: "assinatura",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_pagamento_AssinaturaId",
                table: "pagamento",
                column: "AssinaturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pagamento");

            migrationBuilder.DropTable(
                name: "assinatura");

            migrationBuilder.DropTable(
                name: "plano");
        }
    }
}
