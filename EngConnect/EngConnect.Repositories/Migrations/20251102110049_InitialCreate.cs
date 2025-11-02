using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EngConnect.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_user",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_application_user_UserId",
                        column: x => x.UserId,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_application_user_UserId",
                        column: x => x.UserId,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_application_user_UserId",
                        column: x => x.UserId,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    learner_id = table.Column<string>(type: "text", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'pending'::character varying"),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.order_id);
                    table.ForeignKey(
                        name: "orders_learner_id_fkey",
                        column: x => x.learner_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tutor_profile",
                columns: table => new
                {
                    tutor_id = table.Column<string>(type: "text", nullable: false),
                    experience_years = table.Column<int>(type: "integer", nullable: true),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    profile_picture_url = table.Column<string>(type: "text", nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cv_file = table.Column<string>(type: "text", nullable: true),
                    demo_video = table.Column<string>(type: "text", nullable: true),
                    approved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreateBy = table.Column<string>(type: "text", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tutorprofile_pkey", x => x.tutor_id);
                    table.ForeignKey(
                        name: "tutorprofile_tutor_id_fkey",
                        column: x => x.tutor_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_application_user_UserId",
                        column: x => x.UserId,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    transaction_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transaction_date = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_pkey", x => x.payment_id);
                    table.ForeignKey(
                        name: "payment_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tutor_id = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    total_sessions = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    intro_video = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'pending'::character varying"),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("course_pkey", x => x.course_id);
                    table.ForeignKey(
                        name: "course_tutor_id_fkey",
                        column: x => x.tutor_id,
                        principalTable: "tutor_profile",
                        principalColumn: "tutor_id");
                });

            migrationBuilder.CreateTable(
                name: "tutor_weekly_availability",
                columns: table => new
                {
                    availability_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tutor_id = table.Column<string>(type: "text", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tutorweeklyavailability_pkey", x => x.availability_id);
                    table.ForeignKey(
                        name: "tutorweeklyavailability_tutor_id_fkey",
                        column: x => x.tutor_id,
                        principalTable: "tutor_profile",
                        principalColumn: "tutor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "withdraw_request",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tutor_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    payout_info = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'pending'::character varying"),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("withdrawrequest_pkey", x => x.request_id);
                    table.ForeignKey(
                        name: "withdrawrequest_tutor_id_fkey",
                        column: x => x.tutor_id,
                        principalTable: "tutor_profile",
                        principalColumn: "tutor_id");
                });

            migrationBuilder.CreateTable(
                name: "course_module",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    position = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("coursemodule_pkey", x => x.module_id);
                    table.ForeignKey(
                        name: "coursemodule_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "course_id");
                });

            migrationBuilder.CreateTable(
                name: "course_review",
                columns: table => new
                {
                    course_review_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    learner_id = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("course_review_pkey", x => x.course_review_id);
                    table.ForeignKey(
                        name: "course_review_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "course_review_learner_id_fkey",
                        column: x => x.learner_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "enrollment",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    learner_id = table.Column<string>(type: "text", nullable: false),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    sessions_purchased = table.Column<int>(type: "integer", nullable: false),
                    sessions_remaining = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'active'::character varying"),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("enrollment_pkey", x => x.id);
                    table.ForeignKey(
                        name: "enrollment_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "enrollment_learner_id_fkey",
                        column: x => x.learner_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    sessions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orderitem_pkey", x => x.item_id);
                    table.ForeignKey(
                        name: "orderitem_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "orderitem_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "tutor_schedule",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tutor_id = table.Column<string>(type: "text", nullable: false),
                    availability_id = table.Column<int>(type: "integer", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    is_booked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tutorschedule_pkey", x => x.schedule_id);
                    table.ForeignKey(
                        name: "tutorschedule_availability_id_fkey",
                        column: x => x.availability_id,
                        principalTable: "tutor_weekly_availability",
                        principalColumn: "availability_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "tutorschedule_tutor_id_fkey",
                        column: x => x.tutor_id,
                        principalTable: "tutor_profile",
                        principalColumn: "tutor_id");
                });

            migrationBuilder.CreateTable(
                name: "lesson",
                columns: table => new
                {
                    lesson_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_url = table.Column<string>(type: "text", nullable: true),
                    lesson_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    position = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lesson_pkey", x => x.lesson_id);
                    table.ForeignKey(
                        name: "lesson_module_id_fkey",
                        column: x => x.module_id,
                        principalTable: "course_module",
                        principalColumn: "module_id");
                });

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    enrollment_id = table.Column<int>(type: "integer", nullable: false),
                    schedule_id = table.Column<int>(type: "integer", nullable: true),
                    session_number = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    meeting_link = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'booked'::character varying"),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("session_pkey", x => x.session_id);
                    table.ForeignKey(
                        name: "session_enrollment_id_fkey",
                        column: x => x.enrollment_id,
                        principalTable: "enrollment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "session_schedule_id_fkey",
                        column: x => x.schedule_id,
                        principalTable: "tutor_schedule",
                        principalColumn: "schedule_id");
                });

            migrationBuilder.CreateTable(
                name: "assignment",
                columns: table => new
                {
                    assignment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("assignment_pkey", x => x.assignment_id);
                    table.ForeignKey(
                        name: "assignment_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "assignment_session_id_fkey",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "session_id");
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    learner_id = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("review_pkey", x => x.review_id);
                    table.ForeignKey(
                        name: "review_learner_id_fkey",
                        column: x => x.learner_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "review_session_id_fkey",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "session_id");
                });

            migrationBuilder.CreateTable(
                name: "submission",
                columns: table => new
                {
                    submission_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    assignment_id = table.Column<int>(type: "integer", nullable: false),
                    learner_id = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'not_submitted'::character varying"),
                    score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    feedback = table.Column<string>(type: "text", nullable: true),
                    graded_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("submission_pkey", x => x.submission_id);
                    table.ForeignKey(
                        name: "submission_assignment_id_fkey",
                        column: x => x.assignment_id,
                        principalTable: "assignment",
                        principalColumn: "assignment_id");
                    table.ForeignKey(
                        name: "submission_learner_id_fkey",
                        column: x => x.learner_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "application_user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "application_user",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_assignment_course_id",
                table: "assignment",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_assignment_session_id",
                table: "assignment",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_tutor_id",
                table: "course",
                column: "tutor_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_module_course_id",
                table: "course_module",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_review_course_id",
                table: "course_review",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_review_learner_id",
                table: "course_review",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_course_id",
                table: "enrollment",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_learner_id",
                table: "enrollment",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_module_id",
                table: "lesson",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_course_id",
                table: "order_item",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_order_id",
                table: "order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_learner_id",
                table: "orders",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_order_id",
                table: "payment",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_learner_id",
                table: "review",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_session_id",
                table: "review",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_enrollment_id",
                table: "session",
                column: "enrollment_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_schedule_id",
                table: "session",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_assignment_id",
                table: "submission",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_learner_id",
                table: "submission",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "IX_tutor_schedule_availability_id",
                table: "tutor_schedule",
                column: "availability_id");

            migrationBuilder.CreateIndex(
                name: "IX_tutor_schedule_tutor_id",
                table: "tutor_schedule",
                column: "tutor_id");

            migrationBuilder.CreateIndex(
                name: "IX_tutor_weekly_availability_tutor_id",
                table: "tutor_weekly_availability",
                column: "tutor_id");

            migrationBuilder.CreateIndex(
                name: "IX_withdraw_request_tutor_id",
                table: "withdraw_request",
                column: "tutor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "course_review");

            migrationBuilder.DropTable(
                name: "lesson");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "submission");

            migrationBuilder.DropTable(
                name: "withdraw_request");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "course_module");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "assignment");

            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "enrollment");

            migrationBuilder.DropTable(
                name: "tutor_schedule");

            migrationBuilder.DropTable(
                name: "course");

            migrationBuilder.DropTable(
                name: "tutor_weekly_availability");

            migrationBuilder.DropTable(
                name: "tutor_profile");

            migrationBuilder.DropTable(
                name: "application_user");
        }
    }
}
