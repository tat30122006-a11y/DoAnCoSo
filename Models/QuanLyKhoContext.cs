using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoAnCoSo.Models;

public partial class QuanLyKhoContext : DbContext
{
    public QuanLyKhoContext()
    {
    }

    public QuanLyKhoContext(DbContextOptions<QuanLyKhoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<InboundOrder> InboundOrders { get; set; }

    public virtual DbSet<InboundOrderDetail> InboundOrderDetails { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual DbSet<OutboundOrder> OutboundOrders { get; set; }

    public virtual DbSet<OutboundOrderDetail> OutboundOrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<StockLoss> StockLosses { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TransferOrder> TransferOrders { get; set; }

    public virtual DbSet<TransferOrderDetail> TransferOrderDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VDailyInventoryLedgerReport> VDailyInventoryLedgerReports { get; set; }

    public virtual DbSet<VInventoryDashboardAlert> VInventoryDashboardAlerts { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC07681CD4C3");

            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuditLogs__UserI__5EBF139D");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC0751292204");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<InboundOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InboundO__3214EC07E2BB8366");

            entity.HasIndex(e => e.Status, "IX_InboundOrders_Status");

            entity.HasIndex(e => e.InboundCode, "UQ__InboundO__D6CE9D262AE47737").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InboundCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.ProcessedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("chờ nhập");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InboundOrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InboundOr__Creat__778AC167");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.InboundOrderProcessedByNavigations)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK__InboundOr__Proce__787EE5A0");

            entity.HasOne(d => d.Supplier).WithMany(p => p.InboundOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InboundOr__Suppl__76969D2E");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InboundOrders)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InboundOr__Wareh__75A278F5");
        });

        modelBuilder.Entity<InboundOrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InboundO__3214EC071A4610A9");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.InboundOrder).WithMany(p => p.InboundOrderDetails)
                .HasForeignKey(d => d.InboundOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InboundOr__Inbou__7D439ABD");

            entity.HasOne(d => d.Product).WithMany(p => p.InboundOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InboundOr__Produ__7E37BEF6");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseId, e.ProductId }).HasName("PK__Inventor__ED486395537753DA");

            entity.ToTable("Inventory");

            entity.Property(e => e.MaxQuantity).HasDefaultValue(1000);
            entity.Property(e => e.MinQuantity).HasDefaultValue(10);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__66603565");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__656C112C");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC078F1945B0");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReferenceCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Creat__6E01572D");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__6D0D32F4");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__6C190EBB");
        });

        modelBuilder.Entity<OutboundOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Outbound__3214EC07380028E5");

            entity.HasIndex(e => e.Status, "IX_OutboundOrders_Status");

            entity.HasIndex(e => e.OutboundCode, "UQ__Outbound__1E23CBD3C6081CB4").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.OutboundCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProcessedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("chờ xuất");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OutboundOrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutboundO__Creat__06CD04F7");

            entity.HasOne(d => d.Customer).WithMany(p => p.OutboundOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutboundO__Custo__05D8E0BE");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.OutboundOrderProcessedByNavigations)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK__OutboundO__Proce__07C12930");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.OutboundOrders)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutboundO__Wareh__04E4BC85");
        });

        modelBuilder.Entity<OutboundOrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Outbound__3214EC07E9DA149D");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.OutboundOrder).WithMany(p => p.OutboundOrderDetails)
                .HasForeignKey(d => d.OutboundOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutboundO__Outbo__0C85DE4D");

            entity.HasOne(d => d.Product).WithMany(p => p.OutboundOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutboundO__Produ__0D7A0286");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07C93993B3");

            entity.HasIndex(e => e.Barcode, "IX_Products_Barcode");

            entity.HasIndex(e => e.Sku, "IX_Products_SKU");

            entity.HasIndex(e => e.Barcode, "UQ__Products__177800D3595DC560").IsUnique();

            entity.HasIndex(e => e.Sku, "UQ__Products__CA1ECF0D35000B32").IsUnique();

            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
        });

        modelBuilder.Entity<StockLoss>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockLos__3214EC072A403CC2");

            entity.Property(e => e.Reason).HasMaxLength(250);

            entity.HasOne(d => d.Product).WithMany(p => p.StockLosses)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockLoss__Produ__236943A5");

            entity.HasOne(d => d.TransferOrder).WithMany(p => p.StockLosses)
                .HasForeignKey(d => d.TransferOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockLoss__Trans__22751F6C");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC079FE0E9C0");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TransferOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transfer__3214EC07D672377C");

            entity.ToTable(tb => tb.HasTrigger("trg_OnTransferStatusChange"));

            entity.HasIndex(e => e.Status, "IX_TransferOrders_Status");

            entity.HasIndex(e => e.TransferCode, "UQ__Transfer__CE99A4C573DE3336").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProcessedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("chờ duyệt");
            entity.Property(e => e.TransferCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TransferOrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferO__Creat__160F4887");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.TransferOrderFromWarehouses)
                .HasForeignKey(d => d.FromWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferO__FromW__14270015");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.TransferOrderProcessedByNavigations)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK__TransferO__Proce__17036CC0");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.TransferOrderToWarehouses)
                .HasForeignKey(d => d.ToWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferO__ToWar__151B244E");
        });

        modelBuilder.Entity<TransferOrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transfer__3214EC07F38A106F");

            entity.HasOne(d => d.Product).WithMany(p => p.TransferOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferO__Produ__1CBC4616");

            entity.HasOne(d => d.TransferOrder).WithMany(p => p.TransferOrderDetails)
                .HasForeignKey(d => d.TransferOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TransferO__Trans__1BC821DD");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC073C96BE5A");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4BD5DFBB4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105348AEC34DA").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VDailyInventoryLedgerReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_DailyInventoryLedgerReport");

            entity.Property(e => e.ReferenceCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VInventoryDashboardAlert>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_InventoryDashboardAlert");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
            entity.Property(e => e.StockStatusText).HasMaxLength(50);
            entity.Property(e => e.WarehouseName).HasMaxLength(150);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3214EC077D1FD2B6");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
