using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Linq;

namespace EFCORE
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // CreateDatabase();
            //Console.ReadKey();

            Console.WriteLine($"Insert data base");
            //InsertProduct();

            Console.ReadKey();
            Console.WriteLine($"Read data base");
           // ReadProducts();
            Console.ReadKey();

            Console.WriteLine($"xóa một bản ghi");
           // await DeleteProduct(2);
            Console.ReadKey();

            Console.WriteLine($"update một bản ghi");
            await RenameProduct(3, "update");
            Console.ReadKey();

        }


        public static async void CreateDatabase()
        {
            using (var dbcontext = new ProductsContext())
            {
                bool result = await dbcontext.Database.EnsureCreatedAsync();
                string resultstring = result ? "tạo  thành  công" : "đã có trước đó";
                Console.WriteLine($"CSDL - {resultstring}");

               
            }
        }



        public class ProductsContext : DbContext
        {
            // Chuỗi kết nối, có chỉ định tên CSDL sẽ làm việc
            private const string connectionString = "Data Source=DESKTOP-4DCOTR8\\SQL2014;Initial Catalog=EF;User ID=SA;Password=123456";

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);
                optionsBuilder.UseSqlServer(connectionString);
            }
             public DbSet<Product> products { set; get; }   // Bảng Products trong DB
        }




        [Table("Products")]
        public class Product
        {
            [Key]
            public int ProductId { set; get; }

            [Required]
            [StringLength(50)]
            public string Name { set; get; }

            [StringLength(50)]
            public string Provider { set; get; }
           
        }


        // Thực hiện chèn dữ liệu mẫu, 2 sản phẩm
        public static async void InsertProduct()
        {
            using (var context = new ProductsContext())
            {
                // Dùng đối tượng DbSet để thêm
                await context.products.AddAsync(new Product
                {
                    Name = "Sản phẩm 1",
                    Provider = "Công ty 1"
                });

                // Dùng context để thêm
                await context.AddAsync(new Product()
                {
                    Name = "Sản phẩm 2",
                    Provider = "Công ty 1"
                });


                // Thực hiện Insert vào DB các dữ liệu đã thêm.
                int rows = await context.SaveChangesAsync();
                Console.WriteLine($"Đã lưu {rows} sản phẩm");
            }
        }

        // Xóa database
        public static async void DeleteDatabase()
        {
            Console.Write("Có chắc chắn xóa DB (y) ? ");
            string input = Console.ReadLine();
            if (input.ToLower() == "y")
            {
                using (var context = new ProductsContext())
                {
                    bool deleted = await context.Database.EnsureDeletedAsync();
                    string deletionInfo = deleted ? "đã xóa db" : "không xóa được db";
                    Console.WriteLine($"{deletionInfo}");
                }
            }
        }


        public static async void ReadProducts()
        {
            using (var context = new ProductsContext())
            {
                // Lấy List các sản phẩm từ DB
                var products = await context.products.ToListAsync();
                Console.WriteLine("Tất cả sản phẩm");
                foreach (var product in products)
                {
                    Console.WriteLine($"{product.ProductId,2} {product.Name,10} - {product.Provider}");
                }
                Console.WriteLine();
                Console.WriteLine();

                //Sử dụng LINQ trên DbSet (products)
                products = await (from p in context.products  select p ).ToListAsync();

                Console.WriteLine("Sản phẩm CTY A");
                foreach (var product in products)
                {
                    Console.WriteLine($"{product.ProductId,2} {product.Name,10} - {product.Provider}");
                }
            }
        }


        // Đổi tên sản phẩm có ProductID thành  tên mới
        public static async Task RenameProduct(int id, string newName)
        {
            using (var context = new ProductsContext())
            {

                // context.SetLogging();
                var product = await (from p in context.products
                                     where (p.ProductId == id)
                                     select p
                                 )
                                .FirstOrDefaultAsync(); // Lấy  Product có  ID  chỉ  ra

                if (product != null)
                {
                    product.Name = newName;
                    Console.WriteLine($"{product.ProductId,2} có tên mới = {product.Name,10}");
                    await context.SaveChangesAsync();  //Thi hành cập nhật
                }
            }
        }

        // Xóa sản phẩm có ProductID = id
        public static async Task DeleteProduct(int id)
        {
            using (var context = new ProductsContext())
            {
                // context.SetLogging();
                var product = await (from p in context.products
                                     where (p.ProductId == id)
                                     select p
                                 )
                                .FirstOrDefaultAsync(); // Lấy  Product có  ID  chỉ  ra

                if (product != null)
                {
                    context.Remove(product);
                    Console.WriteLine($"Xóa {product.ProductId}");
                    await context.SaveChangesAsync();
                }
            }
        }



    }
}
