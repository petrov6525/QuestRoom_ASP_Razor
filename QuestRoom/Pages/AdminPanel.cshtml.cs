using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestRoom.Models;

namespace QuestRoom.Pages
{
    [IgnoreAntiforgeryToken]
    public class AdminPanelModel : PageModel
    {
        public AdminPanelModel(ApplicationContext db)
        {
            context = db;


        }




        public ApplicationContext? context;


        [BindProperty]
        public Room? Room { get; set; } = new();





        public List<Image> Images { get; set; } = new();

        public List<Room> Rooms { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Rooms = await context.Rooms.ToListAsync();
            Images = await context.Images.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {


            




            IFormFileCollection files = Request.Form.Files;
            var uploadPath = $"{Directory.GetCurrentDirectory()}/wwwroot/Files";
            Directory.CreateDirectory(uploadPath);

            DirectoryInfo RoomCatalog = new DirectoryInfo($"{uploadPath}/{Room.Name.Replace(" ", "")}");
            if (!RoomCatalog.Exists)
            {
                RoomCatalog.Create();
            }

            string Logo = $"{Room.Name.Replace(" ", "_")}/{files[0].FileName}";
            Room.LogoPath = Logo;


            string LogoPath = $"{uploadPath}/{Logo}";


            using (var fs = new FileStream(LogoPath, FileMode.Create))
            {
                await files[0].CopyToAsync(fs);
            }






            string Image = $"{Room.Name.Replace(" ", "_")}/{files[1].FileName}";



            string ImagePath = $"{uploadPath}/{Image}";


            using (var fs = new FileStream(ImagePath, FileMode.Create))
            {
                await files[1].CopyToAsync(fs);
            }



            await context.Images.AddAsync(new() { Path = Image, Room = Room });



            await context.Rooms.AddAsync(Room!);


            await context.SaveChangesAsync();

            return RedirectToPage("AdminPanel");
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            Room? room = context.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }


            DeleteImagesByRoomId(id);
            DeletePhonesByRoomId(id);

            context.Rooms.Remove(room);

            await context.SaveChangesAsync();

            await AddEditModel.DeleteFiles(room.Name.Replace(" ", "_"));

            return RedirectToPage("AdminPanel");
        }

        private void DeleteImagesByRoomId(int id)
        {
            var images = context.Images.Where(i => i.RoomId == id);

            context.Images.RemoveRange(images);

            context.SaveChanges();
        }

        private void DeletePhonesByRoomId(int id)
        {
            var phones = context.Phones.Where(p => p.RoomId == id);

            context.RemoveRange(phones);

            context.SaveChanges();
        }
    }
}
