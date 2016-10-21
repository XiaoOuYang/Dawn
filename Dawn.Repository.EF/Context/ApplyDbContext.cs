using Dawn.Domain.Entity;
using System.Data.Entity;

namespace Dawn.Repository.EF
{
    public class ApplyDbContext : DbContext
    {
        public ApplyDbContext() : base()
        //: base()
        {
            Database.SetInitializer<ApplyDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JsPermissionApply>()
                .HasKey(x => x.Id)
                .ToTable("JsPermissionApplys");
            modelBuilder.Entity<JsPermissionApply>()
                .Property(x => x.Reason).HasMaxLength(3000).IsRequired();
            modelBuilder.Entity<JsPermissionApply>()
                .Property(x => x.Ip).HasMaxLength(50);
            modelBuilder.Entity<JsPermissionApply>()
                .Property(x => x.ReplyContent).HasMaxLength(3000);
            modelBuilder.Entity<JsPermissionApply>()
                .Property(x => x.User.DisplayName).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<JsPermissionApply>()
               .Property(x => x.User.Alias).HasMaxLength(50).IsRequired();

            modelBuilder.Entity<BlogChangeApply>()
               .HasKey(x => x.Id)
               .ToTable("BlogChangeApplys");
            modelBuilder.Entity<BlogChangeApply>()
                .Property(x => x.Reason).HasMaxLength(3000).IsRequired();
            modelBuilder.Entity<BlogChangeApply>()
                .Property(x => x.TargetBlogApp).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<BlogChangeApply>()
                .Property(x => x.Ip).HasMaxLength(50);
            modelBuilder.Entity<BlogChangeApply>()
                .Property(x => x.ReplyContent).HasMaxLength(3000);
            modelBuilder.Entity<BlogChangeApply>()
                .Property(x => x.User.DisplayName).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<BlogChangeApply>()
               .Property(x => x.User.Alias).HasMaxLength(50).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
