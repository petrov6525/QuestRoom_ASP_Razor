using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestRoom.Models;

namespace QuestRoom.Pages
{
    public class RoomInfoModel : PageModel
    {
        public RoomInfoModel(ApplicationContext db)
        {
            context = db;


        }




        public ApplicationContext? context;


        [BindProperty]
        public Room? Room { get; set; }

        public List<Image> Images { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == id);

            if (Room == null) return NotFound();

            Images =await context.Images.Where(i => i.RoomId == id).ToListAsync();

            return Page();
        }
    }
}
