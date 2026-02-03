using EngConnect.Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Data
{
    public class EngConnectContext : IdentityDbContext<ApplicationUser>
    {
        public EngConnectContext(DbContextOptions<EngConnectContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AssignmentId).HasName("assignment_pkey");

                entity.ToTable("assignment");

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.DueDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("due_date");
                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Course).WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("assignment_course_id_fkey");

                entity.HasOne(d => d.Session).WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("assignment_session_id_fkey");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId).HasName("course_pkey");

                entity.ToTable("course");

                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.IntroVideo).HasColumnName("intro_video");
                entity.Property(e => e.Level)
                    .HasMaxLength(50)
                    .HasColumnName("level");
                entity.Property(e => e.Price)
                    .HasPrecision(12, 2)
                    .HasColumnName("price");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'pending'::character varying")
                    .HasColumnName("status");
                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");
                entity.Property(e => e.TotalSessions).HasColumnName("total_sessions");
                entity.Property(e => e.TutorId).HasColumnName("tutor_id");

                entity.HasOne(d => d.Tutor).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.TutorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_tutor_id_fkey");
            });

            modelBuilder.Entity<CourseModule>(entity =>
            {
                entity.HasKey(e => e.ModuleId).HasName("coursemodule_pkey");

                entity.ToTable("course_module");

                entity.Property(e => e.ModuleId).HasColumnName("module_id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Position).HasColumnName("position");
                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Course).WithMany(p => p.CourseModules)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("coursemodule_course_id_fkey");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("enrollment_pkey");

                entity.ToTable("enrollment");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.LearnerId).HasColumnName("learner_id");
                //entity.Property(e => e.PurchaseDate)
                //    .HasDefaultValueSql("now()")
                //    .HasColumnType("timestamp without time zone")
                //    .HasColumnName("purchase_date");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.SessionsPurchased).HasColumnName("sessions_purchased");
                entity.Property(e => e.SessionsRemaining).HasColumnName("sessions_remaining");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'active'::character varying")
                    .HasColumnName("status");

                entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("enrollment_course_id_fkey");

                entity.HasOne(d => d.Learner).WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.LearnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("enrollment_learner_id_fkey");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.LessonId).HasName("lesson_pkey");

                entity.ToTable("lesson");

                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.ContentUrl).HasColumnName("content_url");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.LessonType)
                    .HasMaxLength(50)
                    .HasColumnName("lesson_type");
                entity.Property(e => e.ModuleId).HasColumnName("module_id");
                entity.Property(e => e.Position).HasColumnName("position");
                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Module).WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("lesson_module_id_fkey");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("orders_pkey");

                entity.ToTable("orders");

                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.LearnerId).HasColumnName("learner_id");
                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(20)
                    .HasColumnName("payment_method");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'pending'::character varying")
                    .HasColumnName("status");
                entity.Property(e => e.TotalAmount)
                    .HasPrecision(12, 2)
                    .HasColumnName("total_amount");

                entity.HasOne(d => d.Learner).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.LearnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orders_learner_id_fkey");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.ItemId).HasName("orderitem_pkey");

                entity.ToTable("order_item");

                entity.Property(e => e.ItemId).HasColumnName("item_id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.Price)
                    .HasPrecision(12, 2)
                    .HasColumnName("price");

                entity.Property(e => e.Sessions).HasColumnName("sessions");

                entity.HasOne(d => d.Course).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orderitem_course_id_fkey");

                entity.HasOne(d => d.Order).WithMany(p => p.Orderitems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orderitem_order_id_fkey");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("payment_pkey");

                entity.ToTable("payment");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");
                entity.Property(e => e.Amount)
                    .HasPrecision(12, 2)
                    .HasColumnName("amount");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");
                entity.Property(e => e.TransactionCode)
                    .HasMaxLength(100)
                    .HasColumnName("transaction_code");
                entity.Property(e => e.TransactionDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("transaction_date");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");

                entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("payment_order_id_fkey");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId).HasName("review_pkey");

                entity.ToTable("review");

                entity.Property(e => e.ReviewId).HasColumnName("review_id");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.LearnerId).HasColumnName("learner_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.SessionId).HasColumnName("session_id");

                entity.HasOne(d => d.Learner).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.LearnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("review_learner_id_fkey");

                entity.HasOne(d => d.Session).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("review_session_id_fkey");
            });
            modelBuilder.Entity<CourseReview>(entity =>
            {
                entity.HasKey(e => e.CourseReviewId).HasName("course_review_pkey");

                entity.ToTable("course_review");

                entity.Property(e => e.CourseReviewId).HasColumnName("course_review_id");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.LearnerId).HasColumnName("learner_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.HasOne(d => d.Learner).WithMany(p => p.CourseReviews)
                    .HasForeignKey(d => d.LearnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_review_learner_id_fkey");

                entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_review_course_id_fkey");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.SessionId).HasName("session_pkey");

                entity.ToTable("session");

                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.EndTime)
                    .HasColumnType("timestamptz")
                    .HasColumnName("end_time");
                entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
                entity.Property(e => e.MeetingLink).HasColumnName("meeting_link");
                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
                entity.Property(e => e.SessionNumber).HasColumnName("session_number");
                entity.Property(e => e.StartTime)
                    .HasColumnType("timestamptz")
                    .HasColumnName("start_time");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'booked'::character varying")
                    .HasColumnName("status");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");

                entity.HasOne(d => d.Enrollment).WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.EnrollmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("session_enrollment_id_fkey");

                entity.HasOne(d => d.Schedule).WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.ScheduleId)
                    .HasConstraintName("session_schedule_id_fkey");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => e.SubmissionId).HasName("submission_pkey");

                entity.ToTable("submission");

                entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.Feedback).HasColumnName("feedback");
                entity.Property(e => e.GradedAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("graded_at");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.LearnerId).HasColumnName("learner_id");
                entity.Property(e => e.Score)
                    .HasPrecision(5, 2)
                    .HasColumnName("score");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'not_submitted'::character varying")
                    .HasColumnName("status");
                entity.Property(e => e.SubmittedAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("submitted_at");

                entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("submission_assignment_id_fkey");

                entity.HasOne(d => d.Learner).WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.LearnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("submission_learner_id_fkey");
            });

            modelBuilder.Entity<TutorProfile>(entity =>
            {
                entity.HasKey(e => e.TutorId).HasName("tutorprofile_pkey");

                entity.ToTable("tutor_profile");

                entity.Property(e => e.TutorId)
                    .ValueGeneratedNever()
                    .HasColumnName("tutor_id");
                entity.Property(e => e.Approved)
                    .HasDefaultValue(false)
                    .HasColumnName("approved");
                entity.Property(e => e.Bio).HasColumnName("bio");
                entity.Property(e => e.Nickname).HasColumnName("nickname");
                entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_url");
                entity.Property(e => e.CvFile).HasColumnName("cv_file");
                entity.Property(e => e.DemoVideo).HasColumnName("demo_video");
                entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
                entity.Property(e => e.Language)
                    .HasMaxLength(50)
                    .HasColumnName("language");

                entity.HasOne(d => d.Tutor).WithOne(p => p.TutorProfile)
                    .HasForeignKey<TutorProfile>(d => d.TutorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tutorprofile_tutor_id_fkey");
            });

            modelBuilder.Entity<TutorSchedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId).HasName("tutorschedule_pkey");

                entity.ToTable("tutor_schedule");

                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
                entity.Property(e => e.AvailabilityId).HasColumnName("availability_id");
                entity.Property(e => e.EndTime)
                    .HasColumnType("timestamptz")
                    .HasColumnName("end_time");
                entity.Property(e => e.IsBooked)
                    .HasDefaultValue(false)
                    .HasColumnName("is_booked");
                entity.Property(e => e.StartTime)
                    .HasColumnType("timestamptz")
                    .HasColumnName("start_time");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.TutorId).HasColumnName("tutor_id");

                entity.HasOne(d => d.Availability).WithMany(p => p.Tutorschedules)
                    .HasForeignKey(d => d.AvailabilityId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("tutorschedule_availability_id_fkey");

                entity.HasOne(d => d.Tutor).WithMany(p => p.TutorSchedules)
                    .HasForeignKey(d => d.TutorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tutorschedule_tutor_id_fkey");
            });

            modelBuilder.Entity<TutorWeeklyAvailability>(entity =>
            {
                entity.HasKey(e => e.AvailabilityId).HasName("tutorweeklyavailability_pkey");

                entity.ToTable("tutor_weekly_availability");

                entity.Property(e => e.AvailabilityId).HasColumnName("availability_id");
                entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.StartTime).HasColumnName("start_time");
                entity.Property(e => e.TutorId).HasColumnName("tutor_id");

                entity.HasOne(d => d.Tutor).WithMany(p => p.TutorWeeklyAvailabilities)
                    .HasForeignKey(d => d.TutorId)
                    .HasConstraintName("tutorweeklyavailability_tutor_id_fkey");
            });

            modelBuilder.Entity<WithdrawRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId).HasName("withdrawrequest_pkey");

                entity.ToTable("withdraw_request");

                entity.Property(e => e.RequestId).HasColumnName("request_id");
                entity.Property(e => e.Amount)
                    .HasPrecision(12, 2)
                    .HasColumnName("amount");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.PayoutInfo).HasColumnName("payout_info");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'pending'::character varying")
                    .HasColumnName("status");
                entity.Property(e => e.TutorId).HasColumnName("tutor_id");

                entity.HasOne(d => d.Tutor).WithMany(p => p.WithdrawRequests)
                    .HasForeignKey(d => d.TutorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("withdrawrequest_tutor_id_fkey");
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("application_user");

                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
            });

            modelBuilder.Entity<ChatThread>(entity =>
            {
                entity.HasKey(e => e.ThreadId).HasName("chatthread_pkey");
                entity.ToTable("chat_thread");

                entity.Property(e => e.ThreadId).HasColumnName("thread_id");
                entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
                entity.Property(e => e.IsGroup).HasColumnName("is_group");

                entity.Property(e => e.Note).HasMaxLength(255).HasColumnName("note");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
            });

            modelBuilder.Entity<ChatParticipant>(entity =>
            {
                entity.HasKey(e => e.ParticipantId).HasName("chatparticipant_pkey");
                entity.ToTable("chat_participant");

                entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
                entity.Property(e => e.ThreadId).HasColumnName("thread_id");
                entity.Property(e => e.UserId).HasMaxLength(450).HasColumnName("user_id");
                entity.Property(e => e.JoinedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("joined_at");
                entity.Property(e => e.LastReadAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("last_read_at");
                entity.Property(e => e.IsMuted).HasColumnName("is_muted");

                entity.HasIndex(e => new { e.ThreadId, e.UserId }).IsUnique().HasDatabaseName("ux_chat_participant_thread_user");

                entity.HasOne(d => d.Thread).WithMany(p => p.Participants)
                    .HasForeignKey(d => d.ThreadId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("chatparticipant_thread_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.ChatParticipants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("chatparticipant_user_id_fkey");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId).HasName("chatmessage_pkey");
                entity.ToTable("chat_message");

                entity.Property(e => e.MessageId).HasColumnName("message_id");
                entity.Property(e => e.ThreadId).HasColumnName("thread_id");
                entity.Property(e => e.SenderId).HasMaxLength(450).HasColumnName("sender_id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.ContentType).HasMaxLength(40).HasColumnName("content_type");
                entity.Property(e => e.IsEdited).HasColumnName("is_edited");

                entity.Property(e => e.Note).HasMaxLength(255).HasColumnName("note");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");

                entity.HasIndex(e => new { e.ThreadId, e.CreatedAt }).HasDatabaseName("ix_chat_message_thread_created");

                entity.HasOne(d => d.Thread).WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ThreadId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("chatmessage_thread_id_fkey");

                entity.HasOne(d => d.Sender).WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("chatmessage_sender_id_fkey");
            });

            // Call-related entities start
            modelBuilder.Entity<CallSession>(entity =>
            {
                entity.HasKey(e => e.CallSessionId).HasName("callsession_pkey");

                entity.ToTable("call_session");

                entity.Property(e => e.CallSessionId).HasColumnName("call_session_id");

                entity.Property(e => e.CallerId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("caller_id");

                entity.Property(e => e.CalleeId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("callee_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'pending'::character varying")
                    .HasColumnName("status");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("created_at");

                entity.Property(e => e.StartedAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("started_at");

                entity.Property(e => e.EndedAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("ended_at");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");

                entity.HasOne(d => d.Caller).WithMany(p => p.OutgoingCallSessions)
                    .HasForeignKey(d => d.CallerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("callsession_caller_id_fkey");

                entity.HasOne(d => d.Callee).WithMany(p => p.IncomingCallSessions)
                    .HasForeignKey(d => d.CalleeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("callsession_callee_id_fkey");
            });

            modelBuilder.Entity<CallParticipant>(entity =>
            {
                entity.HasKey(e => e.ParticipantId).HasName("callparticipant_pkey");

                entity.ToTable("call_participant");

                entity.Property(e => e.ParticipantId).HasColumnName("participant_id");

                entity.Property(e => e.CallSessionId).HasColumnName("call_session_id");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.Property(e => e.IsCaller).HasColumnName("is_caller");
                entity.Property(e => e.IsConnected).HasColumnName("is_connected");
                entity.Property(e => e.IsMuted).HasColumnName("is_muted");
                entity.Property(e => e.IsVideoEnabled).HasColumnName("is_video_enabled");

                entity.Property(e => e.JoinedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("joined_at");

                entity.Property(e => e.LeftAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("left_at");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");

                entity.HasIndex(e => new { e.CallSessionId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("ux_call_participant_session_user");

                entity.HasOne(d => d.CallSession).WithMany(p => p.Participants)
                    .HasForeignKey(d => d.CallSessionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("callparticipant_call_session_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.CallParticipants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("callparticipant_user_id_fkey");
            });

            modelBuilder.Entity<CallSignal>(entity =>
            {
                entity.HasKey(e => e.SignalId).HasName("callsignal_pkey");

                entity.ToTable("call_signal");

                entity.Property(e => e.SignalId).HasColumnName("signal_id");

                entity.Property(e => e.CallSessionId).HasColumnName("call_session_id");

                entity.Property(e => e.SenderId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("sender_id");

                entity.Property(e => e.ReceiverId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("receiver_id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("type");

                entity.Property(e => e.Payload)
                    .IsRequired()
                    .HasColumnName("payload");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("created_at");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");

                entity.HasIndex(e => new { e.CallSessionId, e.CreatedAt })
                    .HasDatabaseName("ix_call_signal_session_created");

                entity.HasOne(d => d.CallSession).WithMany(p => p.Signals)
                    .HasForeignKey(d => d.CallSessionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("callsignal_call_session_id_fkey");

                entity.HasOne(d => d.Sender).WithMany(p => p.SentCallSignals)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("callsignal_sender_id_fkey");

                entity.HasOne(d => d.Receiver).WithMany(p => p.ReceivedCallSignals)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("callsignal_receiver_id_fkey");
            });
            // Call-related entities end

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("refreshtoken_pkey");

                entity.ToTable("refresh_token");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("token");
                entity.Property(e => e.JwtId)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("jwt_id");
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");
                entity.Property(e => e.ExpiresAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("expires_at");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamptz")
                    .HasColumnName("created_at");
                entity.Property(e => e.RevokedAt)
                    .HasColumnType("timestamptz")
                    .HasColumnName("revoked_at");
                entity.Property(e => e.IsRevoked)
                    .HasColumnName("is_revoked");
                entity.Property(e => e.IsUsed)
                    .HasColumnName("is_used");

                entity.HasIndex(e => e.Token).IsUnique().HasDatabaseName("ux_refreshtoken_token");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("refreshtoken_user_id_fkey");
            });
        }

        public virtual DbSet<Assignment> Assignments { get; set; }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<CourseModule> CourseModules { get; set; }

        public virtual DbSet<Enrollment> Enrollments { get; set; }

        public virtual DbSet<Lesson> Lessons { get; set; }

        public virtual DbSet<Orders> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<CourseReview> CourseReviews { get; set; }

        public virtual DbSet<Session> Sessions { get; set; }

        public virtual DbSet<Submission> Submissions { get; set; }

        public virtual DbSet<TutorProfile> TutorProfiles { get; set; }

        public virtual DbSet<TutorSchedule> TutorSchedules { get; set; }

        public virtual DbSet<TutorWeeklyAvailability> TutorWeeklyAvailabilities { get; set; }

        public virtual DbSet<WithdrawRequest> WithdrawRequests { get; set; }

        public virtual DbSet<ChatThread> ChatThreads { get; set; }

        public virtual DbSet<ChatParticipant> ChatParticipants { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<CallSession> CallSessions { get; set; }

        public virtual DbSet<CallParticipant> CallParticipants { get; set; }

        public virtual DbSet<CallSignal> CallSignals { get; set; }
    }
}
