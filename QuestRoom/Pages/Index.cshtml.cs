using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using QuestRoom.Models;

namespace QuestRoom.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        public IndexModel(ApplicationContext db)
        {
            context = db;


        }




        public ApplicationContext? context;


        [BindProperty]
        public Room? Room { get; set; } = new();





        public List<Image> Images { get; set; } = new();

        public List<Room> Rooms { get; set; } = new();


        [BindProperty]
        public string? filter { get; set; } = "complexity";

        [BindProperty]
        public string? order { get; set; } = "up";

        public async Task<IActionResult> OnGetAsync()
        {
            Rooms =await  context.Rooms.ToListAsync();
             Images = await context.Images.ToListAsync();

            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            Rooms = await context.Rooms.ToListAsync();
            Images = await context.Images.ToListAsync();


            Rooms = CheckFilters(Request);


            return Page();
        }

        
        private List<Room> CheckFilters(HttpRequest request)
        {
            filter = request.Form["filter"];
            order = request.Form["order"];

            Console.WriteLine("order"+order);

            List<Room> tmpRooms = new List<Room>();

            if (filter.Equals("complexity"))
            {
                tmpRooms = Rooms.OrderBy(r => r.Complexity).ToList();

                if (order.Equals("down"))
                {
                    tmpRooms = Rooms.OrderByDescending(r => r.Complexity).ToList();
                }
            }
            else if (filter.Equals("players"))
            {
                tmpRooms = Rooms.OrderBy(r => r.MaxPLayers).ToList();

                if (order.Equals("down"))
                {
                    tmpRooms = Rooms.OrderByDescending(r => r.MaxPLayers).ToList();
                }
            }
            else if (filter.Equals("scary"))
            {
                tmpRooms = Rooms.OrderBy(r => r.Scary).ToList();

                if (order.Equals("down"))
                {
                    tmpRooms = Rooms.OrderByDescending(r => r.Scary).ToList();
                }
            }


            return tmpRooms;
        }
    }
}