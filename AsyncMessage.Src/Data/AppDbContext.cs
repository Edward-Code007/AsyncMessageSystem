using AsyncMessageSystem.Order.Model;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<OrderModel> Order { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OrderModel>()
        .HasKey(x => x.Id);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder
        .UseNpgsql(@"Host=localhost:5432;Username=admin;Password=admin;Database=default_db")
        .UseSeeding((ctx, flag) =>
        {
            ctx.Set<OrderModel>().AddRange([
               new OrderModel(Guid.CreateVersion7(),"Ruso","Guayaba",10),
               new OrderModel(Guid.CreateVersion7(),"Felix","Pepinos",10),
               new OrderModel(Guid.CreateVersion7(),"Felix","FrutaBomba",15),
            ]);
            ctx.SaveChanges();
        }).UseAsyncSeeding(async (ctx, flag, cancellationToken) =>
        {
            //ctx.Database.EnsureCreated();
            ctx.Set<OrderModel>().AddRange([
               new OrderModel(Guid.CreateVersion7(),"Ruso","Guayaba",10),
               new OrderModel(Guid.CreateVersion7(),"Felix","Pepinos",10),
               new OrderModel(Guid.CreateVersion7(),"Moise","FrutaBomba",15),
            ]);
            await ctx.SaveChangesAsync();
        });

        
    }
}