// namespace Myshop.Application.Services
// {
//     public class CommissionService : ICommissionService
//     {
//         private readonly decimal _defaultPercent;
//         public CommissionService(IConfiguration config)
//         {
//             _defaultPercent = config.GetValue<decimal?>("Marketplace:CommissionPercent") ?? 0.05m;
//         }

//         public decimal CalculateCommission(decimal price, decimal percent)
//         {
//             var p = percent == 0 ? _defaultPercent : percent;
//             return Math.Round(price * p, 2);
//         }
//     }
// }