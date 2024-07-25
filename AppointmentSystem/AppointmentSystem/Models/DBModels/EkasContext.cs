using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace AppointmentSystem.Models.DBModels;

public partial class EkasContext : DbContext
{
    public EkasContext()
    {
    }

    public EkasContext(DbContextOptions<EkasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Appointmenttreatment> Appointmenttreatments { get; set; }

    public virtual DbSet<Arrangedoctorshift> Arrangedoctorshifts { get; set; }

    public virtual DbSet<Arrangemonthshift> Arrangemonthshifts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Customertoken> Customertokens { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Doctordayoff> Doctordayoffs { get; set; }

    public virtual DbSet<Doctoroutpatient> Doctoroutpatients { get; set; }

    public virtual DbSet<Doctortreatment> Doctortreatments { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rolepermission> Rolepermissions { get; set; }

    public virtual DbSet<Shifttype> Shifttypes { get; set; }

    public virtual DbSet<Systemfile> Systemfiles { get; set; }

    public virtual DbSet<Systemlog> Systemlogs { get; set; }

    public virtual DbSet<Systemparameter> Systemparameters { get; set; }

    public virtual DbSet<Systemselectlist> Systemselectlists { get; set; }

    public virtual DbSet<Treatment> Treatments { get; set; }

    public virtual DbSet<Treatmentlabel> Treatmentlabels { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userpermission> Userpermissions { get; set; }

    public virtual DbSet<Verificationcode> Verificationcodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseMySql("server=localhost;port=3306;database=ekas;user=EKSystem;password=Kn6Wp7qu8yMU;charset=utf8", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.4.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("account", tb => tb.HasComment("使用者帳號資料表"));

            entity.HasIndex(e => e.UserId, "FK_account_user");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Account1)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("Account");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_account_user");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("appointment", tb => tb.HasComment("預約資料表"));

            entity.HasIndex(e => e.CustomerId, "FK_appointment_customer");

            entity.HasIndex(e => e.DoctorId, "FK_appointment_doctor");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.BookingBeginTime)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.BookingEndTime)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CheckIn)
                .HasMaxLength(10)
                .HasDefaultValueSql("'N'");
            entity.Property(e => e.CheckInTime)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.CustomerId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Date)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'");

            entity.HasOne(d => d.Customer).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointment_customer");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointment_doctor");
        });

        modelBuilder.Entity<Appointmenttreatment>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("appointmenttreatment", tb => tb.HasComment("預約療程資料表"));

            entity.HasIndex(e => e.AppointmentId, "FK_appointmenttreatment_appointment");

            entity.HasIndex(e => e.TreatmentId, "FK_appointmenttreatment_treatment");

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.AppointmentId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.TreatmentId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValueSql("''")
                .HasComment("A:預約的療程，R:實際執行的療程");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Appointmenttreatments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointmenttreatment_appointment");

            entity.HasOne(d => d.Treatment).WithMany(p => p.Appointmenttreatments)
                .HasForeignKey(d => d.TreatmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointmenttreatment_treatment");
        });

        modelBuilder.Entity<Arrangedoctorshift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("arrangedoctorshift", tb => tb.HasComment("預排醫師班表資料表"));

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Day)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Month)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.ShiftTypeId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Year)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Arrangemonthshift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("arrangemonthshift", tb => tb.HasComment("預排月份班表資料表"));

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Locked)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValueSql("'N'");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Month)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Year)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("customer", tb => tb.HasComment("客戶資料表"));

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Birthday)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CellPhone)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.LineId)
                .HasMaxLength(100)
                .HasDefaultValueSql("''");
            entity.Property(e => e.LinePictureUrl)
                .HasMaxLength(500)
                .HasDefaultValueSql("''");
            entity.Property(e => e.MedicalRecordNumber)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasDefaultValueSql("''");
            entity.Property(e => e.NationalIdNumber)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
        });

        modelBuilder.Entity<Customertoken>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("customertoken", tb => tb.HasComment("客戶Toekn資料表"));

            entity.HasIndex(e => e.CustomerId, "FK_customertoken_customer");

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(500)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.ExpiresIn)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(500)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");

            entity.HasOne(d => d.Customer).WithMany(p => p.Customertokens)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_customertoken_customer");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doctor", tb => tb.HasComment("醫師資料表"));

            entity.HasIndex(e => e.ImageFileId, "FK_doctor_systemfile");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.ColorHex)
                .HasMaxLength(10)
                .HasDefaultValueSql("''")
                .HasColumnName("ColorHEX");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.DepartmentTitle)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.DoctorName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.DoctorNameEnglish)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.ImageFileId).HasMaxLength(50);
            entity.Property(e => e.Introduction)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Sort).HasColumnType("int(11)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");

            entity.HasOne(d => d.ImageFile).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.ImageFileId)
                .HasConstraintName("FK_doctor_systemfile");
        });

        modelBuilder.Entity<Doctordayoff>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("doctordayoff", tb => tb.HasComment("醫師請假資料表"));

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.BeginTime)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Date)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.EndTime)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Doctoroutpatient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doctoroutpatient", tb => tb.HasComment("醫師門診資料表"));

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.AppointmentId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.ArrangeId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.BeginTime)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Day)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.EndTime)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Month)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Year)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Doctortreatment>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("doctortreatment", tb => tb.HasComment("醫師負責療程資料表"));

            entity.HasIndex(e => e.DoctorId, "FK_doctortreatment_doctor");

            entity.HasIndex(e => e.TreatmentId, "FK_doctortreatment_treatment");

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.TreatmentId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Doctortreatments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_doctortreatment_doctor");

            entity.HasOne(d => d.Treatment).WithMany(p => p.Doctortreatments)
                .HasForeignKey(d => d.TreatmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_doctortreatment_treatment");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("function", tb => tb.HasComment("系統功能資料表"));

            entity.HasIndex(e => e.ModuleId, "FK_function_module");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Controller)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.FunctionName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.ModuleId)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Sort).HasColumnType("int(11)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");

            entity.HasOne(d => d.Module).WithMany(p => p.Functions)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_function_module");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("label", tb => tb.HasComment("標籤資料表"));

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.LabelName)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("labelName");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Sort)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasComment("Treatment:療程用的標籤");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("module", tb => tb.HasComment("系統模組資料表"));

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Sort).HasColumnType("int(11)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role", tb => tb.HasComment("角色資料表"));

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
        });

        modelBuilder.Entity<Rolepermission>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("rolepermission", tb => tb.HasComment("角色預設權限資料表"));

            entity.HasIndex(e => e.RoleId, "FK_positionpermission_position");

            entity.HasIndex(e => e.FunctionId, "FK_rolepermission_function");

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.FunctionId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.IsAllow)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");

            entity.HasOne(d => d.Function).WithMany(p => p.Rolepermissions)
                .HasForeignKey(d => d.FunctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rolepermission_function");

            entity.HasOne(d => d.Role).WithMany(p => p.Rolepermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rolepermission_role");
        });

        modelBuilder.Entity<Shifttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shifttype", tb => tb.HasComment("班表類別資料表"));

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.BeginTime)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.EndTime)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Sort).HasColumnType("tinyint(4)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
        });

        modelBuilder.Entity<Systemfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("systemfile", tb => tb.HasComment("上傳檔案資料表"));

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.FileSize)
                .HasDefaultValueSql("'0'")
                .HasColumnType("bigint(20)");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Path)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
        });

        modelBuilder.Entity<Systemlog>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("systemlog", tb => tb.HasComment("系統紀錄資料表"));

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.UserAccount)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Default'");
        });

        modelBuilder.Entity<Systemparameter>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("systemparameter");

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Locked)
                .HasDefaultValueSql("'N'")
                .HasColumnType("text");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Value)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
        });

        modelBuilder.Entity<Systemselectlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("systemselectlist", tb => tb.HasComment("下拉式選單資料表"));

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.SelectName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.SelectValue)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("treatment", tb => tb.HasComment("療程資料表"));

            entity.HasIndex(e => e.ImageFileId, "FK_treatment_systemfile");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.AlertMessage)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Hide)
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.ImageFileId).HasMaxLength(50);
            entity.Property(e => e.Introduction)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Memo)
                .HasDefaultValueSql("''")
                .HasColumnType("text");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Sort).HasColumnType("int(11)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Time).HasColumnType("int(11)");
            entity.Property(e => e.TreatmentName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.ImageFile).WithMany(p => p.Treatments)
                .HasForeignKey(d => d.ImageFileId)
                .HasConstraintName("FK_treatment_systemfile");
        });

        modelBuilder.Entity<Treatmentlabel>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("treatmentlabel", tb => tb.HasComment("療程標籤資料表"));

            entity.HasIndex(e => e.LabelId, "FK_treatmentlable_lable");

            entity.HasIndex(e => e.TreatmentId, "FK_treatmentlable_treatment");

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.LabelId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.TreatmentId)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.Label).WithMany(p => p.Treatmentlabels)
                .HasForeignKey(d => d.LabelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_treatmentlable_lable");

            entity.HasOne(d => d.Treatment).WithMany(p => p.Treatmentlabels)
                .HasForeignKey(d => d.TreatmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_treatmentlable_treatment");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user", tb => tb.HasComment("使用者資料表"));

            entity.HasIndex(e => e.RoleId, "FK_user_position");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Birthday)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasDefaultValueSql("''");
            entity.Property(e => e.IsAdmin)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.Telphone)
                .HasMaxLength(20)
                .HasDefaultValueSql("''");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(200)
                .HasDefaultValueSql("''");
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.UserNameEnglish)
                .HasMaxLength(100)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_user_role");
        });

        modelBuilder.Entity<Userpermission>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("userpermission", tb => tb.HasComment("使用者權限資料表"));

            entity.HasIndex(e => e.FunctionId, "FK_userpermission_function");

            entity.HasIndex(e => e.UserId, "FK_userpermission_user");

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.FunctionId).HasMaxLength(50);
            entity.Property(e => e.IsAllow)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
            entity.Property(e => e.UserId).HasMaxLength(50);

            entity.HasOne(d => d.Function).WithMany(p => p.Userpermissions)
                .HasForeignKey(d => d.FunctionId)
                .HasConstraintName("FK_userpermission_function");

            entity.HasOne(d => d.User).WithMany(p => p.Userpermissions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_userpermission_user");
        });

        modelBuilder.Entity<Verificationcode>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("verificationcode", tb => tb.HasComment("驗證碼資料表"));

            entity.Property(e => e.Index).HasColumnType("bigint(20)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ExpireTime)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.ForeignKey)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.HashCode)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.LoginBy)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Modifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("'Default'");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Otp)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValueSql("''");
            entity.Property(e => e.SouceTable)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(1)
                .HasDefaultValueSql("'Y'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
