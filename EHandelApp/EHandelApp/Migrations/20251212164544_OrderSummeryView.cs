using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHandelApp.Migrations
{
    /// <inheritdoc />
    public partial class OrderSummeryView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS OrderSummeryView AS
            SELECT
                o.OrderId,
                o.OrderDate,
                c.FirstName || ' ' || c.LastName AS CustomerName,
                c.Email AS CustomerEmail,
                IFNULL(SUM(oi.Quantity * oi.UnitPrice), 0) AS TotalAmount
            FROM Orders o
            JOIN Customers c ON o.CustomerId = c.CustomerId
            LEFT JOIN OrderRows oi ON o.OrderId = oi.OrderId
            GROUP BY o.OrderId, o.OrderDate, c.FirstName, c.LastName, c.Email;
            ");

            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Insert
            AFTER INSERT ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = (
                                    SELECT IFNULL (SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows
                                    WHERE OrderId = NEW.OrderId
                                  )
                WHERE OrderId = NEW.OrderId;
            END;
            ");
            
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Update
            AFTER UPDATE ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = (
                                    SELECT IFNULL (SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows
                                    WHERE OrderId = NEW.OrderId
                                  )
                WHERE OrderId = NEW.OrderId;
            END;
            ");
            
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Delete
            AFTER DELETE ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = (
                                    SELECT IFNULL (SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows
                                    WHERE OrderId = OLD.OrderId
                                  )
                WHERE OrderId = OLD.OrderId;
            END;
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             DROP VIEW IF EXISTS OrderSummeryView;
            ");
            migrationBuilder.Sql(@"
             DROP TRIGGER IF EXISTS trg_OrderRow_Insert;
            ");
            migrationBuilder.Sql(@"
             DROP TRIGGER IF EXISTS trg_OrderRow_Update;
            ");
            migrationBuilder.Sql(@"
             DROP TRIGGER IF EXISTS trg_OrderRow_Delete;
            ");
        }
    }
}
