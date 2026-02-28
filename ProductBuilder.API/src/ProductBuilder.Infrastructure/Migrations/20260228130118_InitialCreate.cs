using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "insurers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    license_no = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_insurers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lines_of_business",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lines_of_business", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brokers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    insurer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    company_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    license_no = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    commission_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brokers", x => x.id);
                    table.ForeignKey(
                        name: "FK_brokers_insurers_insurer_id",
                        column: x => x.insurer_id,
                        principalTable: "insurers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_brokers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lob_id = table.Column<Guid>(type: "uuid", nullable: false),
                    insurer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    effective_date = table.Column<DateOnly>(type: "date", nullable: false),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_insurers_insurer_id",
                        column: x => x.insurer_id,
                        principalTable: "insurers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_lines_of_business_lob_id",
                        column: x => x.lob_id,
                        principalTable: "lines_of_business",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "underwriters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    license_no = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    specialization = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    authority_limit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_underwriters", x => x.id);
                    table.ForeignKey(
                        name: "FK_underwriters_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "coverages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    sequence_no = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coverages", x => x.id);
                    table.ForeignKey(
                        name: "FK_coverages_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quotes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    broker_id = table.Column<Guid>(type: "uuid", nullable: true),
                    underwriter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    insured_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    insured_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    insured_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    base_premium = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    total_premium = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    valid_until = table.Column<DateOnly>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotes", x => x.id);
                    table.ForeignKey(
                        name: "FK_quotes_brokers_broker_id",
                        column: x => x.broker_id,
                        principalTable: "brokers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_quotes_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quotes_underwriters_underwriter_id",
                        column: x => x.underwriter_id,
                        principalTable: "underwriters",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_quotes_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "covers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    coverage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    sequence_no = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_covers", x => x.id);
                    table.ForeignKey(
                        name: "FK_covers_coverages_coverage_id",
                        column: x => x.coverage_id,
                        principalTable: "coverages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deductibles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deductible_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    min_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    max_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    default_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deductibles", x => x.id);
                    table.ForeignKey(
                        name: "FK_deductibles_covers_cover_id",
                        column: x => x.cover_id,
                        principalTable: "covers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "limits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: false),
                    limit_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    min_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    max_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    default_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_limits", x => x.id);
                    table.ForeignKey(
                        name: "FK_limits_covers_cover_id",
                        column: x => x.cover_id,
                        principalTable: "covers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modifiers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: true),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    modifier_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    value_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    min_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    max_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    default_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modifiers", x => x.id);
                    table.ForeignKey(
                        name: "FK_modifiers_covers_cover_id",
                        column: x => x.cover_id,
                        principalTable: "covers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_modifiers_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "premiums",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: false),
                    premium_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    base_rate = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: true),
                    flat_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    min_premium = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    calculation_basis = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_premiums", x => x.id);
                    table.ForeignKey(
                        name: "FK_premiums_covers_cover_id",
                        column: x => x.cover_id,
                        principalTable: "covers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quote_covers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quote_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_selected = table.Column<bool>(type: "boolean", nullable: false),
                    selected_limit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    selected_deductible = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    calculated_premium = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    basis_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quote_covers", x => x.id);
                    table.ForeignKey(
                        name: "FK_quote_covers_covers_cover_id",
                        column: x => x.cover_id,
                        principalTable: "covers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quote_covers_quotes_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quote_modifiers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quote_id = table.Column<Guid>(type: "uuid", nullable: false),
                    modifier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    applied_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    premium_impact = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quote_modifiers", x => x.id);
                    table.ForeignKey(
                        name: "FK_quote_modifiers_modifiers_modifier_id",
                        column: x => x.modifier_id,
                        principalTable: "modifiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quote_modifiers_quotes_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "lines_of_business",
                columns: new[] { "id", "code", "created_at", "description", "is_active", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000010"), "PROP", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Property insurance", true, "Property", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "MARINE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Marine insurance", true, "Marine", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "MOTOR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Motor insurance", true, "Motor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Underwriter" },
                    { 3, "Broker" },
                    { 4, "Insurer" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "created_at", "email", "first_name", "is_active", "last_name", "password_hash", "role_id", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@productbuilder.com", "System", true, "Admin", "$2a$11$ZE0pRtvQcTDsoQnDW4d3AOwFtG15eoI/H7Y4Pvv8sJptEDvsICJwe", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_brokers_insurer_id",
                table: "brokers",
                column: "insurer_id");

            migrationBuilder.CreateIndex(
                name: "IX_brokers_user_id",
                table: "brokers",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coverages_product_id",
                table: "coverages",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_covers_coverage_id",
                table: "covers",
                column: "coverage_id");

            migrationBuilder.CreateIndex(
                name: "IX_deductibles_cover_id",
                table: "deductibles",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "IX_insurers_code",
                table: "insurers",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_limits_cover_id",
                table: "limits",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "IX_lines_of_business_code",
                table: "lines_of_business",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modifiers_cover_id",
                table: "modifiers",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "IX_modifiers_product_id",
                table: "modifiers",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_premiums_cover_id",
                table: "premiums",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_code",
                table: "products",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_created_by",
                table: "products",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_products_insurer_id",
                table: "products",
                column: "insurer_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_lob_id",
                table: "products",
                column: "lob_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_covers_cover_id",
                table: "quote_covers",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_covers_quote_id",
                table: "quote_covers",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_modifiers_modifier_id",
                table: "quote_modifiers",
                column: "modifier_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_modifiers_quote_id",
                table: "quote_modifiers",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_broker_id",
                table: "quotes",
                column: "broker_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_created_by",
                table: "quotes",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_product_id",
                table: "quotes",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotes_underwriter_id",
                table: "quotes",
                column: "underwriter_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_underwriters_user_id",
                table: "underwriters",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deductibles");

            migrationBuilder.DropTable(
                name: "limits");

            migrationBuilder.DropTable(
                name: "premiums");

            migrationBuilder.DropTable(
                name: "quote_covers");

            migrationBuilder.DropTable(
                name: "quote_modifiers");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "modifiers");

            migrationBuilder.DropTable(
                name: "quotes");

            migrationBuilder.DropTable(
                name: "covers");

            migrationBuilder.DropTable(
                name: "brokers");

            migrationBuilder.DropTable(
                name: "underwriters");

            migrationBuilder.DropTable(
                name: "coverages");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "insurers");

            migrationBuilder.DropTable(
                name: "lines_of_business");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
