using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestRoom.Models;

namespace QuestRoom.Pages
{
    [IgnoreAntiforgeryToken]
    public class AddEditModel : PageModel
    {
        public AddEditModel(ApplicationContext db)
        {
            context = db;
        }

        public ApplicationContext context;


        [BindProperty]
        public Room Room { get; set; } = new();



        public List<Room> Rooms { get; set; } = new();


        public string? Name { get; set; }
        


        public async Task<IActionResult> OnGetAsync(int id)
        {
            Room = context.Rooms.FirstOrDefault(r => r.Id == id);

            var room = context.Rooms.FirstOrDefault(r => r.Id == id);

            Name = room?.Name;


            Console.WriteLine(Name);

            return Page();
        }

        



        public async Task<IActionResult> OnPostAsync()
        {
            Room.Name = Room.Name?.Replace(" ", "_");


            //Create
            if (Room.Id == 0)
            {
                await LoadFiles(Request, Room, context!);


                

                context.Rooms.Add(Room!);

                UpdatePhones();
            }
            //Update
            else
            {
                var oldRoom = context.Rooms.AsNoTracking().FirstOrDefault(r => r.Id == Room.Id);
                string oldName = oldRoom.Name;

                Console.WriteLine("Old - " + oldName);


                string tmpName = Room.Name;

                Room.Name = oldName;


                await UpdateLogo();

                UpdateImages();
                await LoadImages(Request, Room, context);

                Room.Name = tmpName;

                if (tmpName != oldName)
                {
                    RenameDirectory(oldName!,tmpName!);
                }


                RemovePhones();
                UpdatePhones();


                
                context.Rooms.Update(Room);

            }
            await context.SaveChangesAsync();

            await DeleteImages();

            return RedirectToPage("AdminPanel");

        }





        private async Task DeleteImages()
        {
            var images = context.Images.Where(i => i.RoomId == Room.Id);
            var Logo = Room.LogoPath;

            string dirName = $"{Directory.GetCurrentDirectory()}/wwwroot/Files/{Room.Name}";

            string[] files;

            if (Directory.Exists(dirName))
            {
                files = Directory.GetFiles(dirName);

                if (files != null)
                {
                    //foreach (var image in images)
                    //{
                    //    foreach (var file in files)
                    //    {
                    //        bool isExist = false;

                    //        var list = file.Split("\\");
                    //        var filePath = list[list.Length-1];

                    //        if (filePath.Equals(image.Path) || filePath.Equals(Logo))
                    //        {
                    //            isExist = true;
                    //        }

                    //        if (!isExist)
                    //        {
                    //            var fileInfo = new FileInfo(file);

                    //            if (fileInfo.Exists)
                    //            {
                    //                //fileInfo.Delete();
                    //            }
                    //        }

                    //    }
                    //}



                    foreach (var file in files)
                    {
                        bool isExists = false;

                        var list = file.Split("\\");
                        var filePath = list[list.Length - 1];

                        foreach (var image in images)
                        {
                            if (filePath.Equals(image.Path) || filePath.Equals(Logo))
                            {
                                isExists = true;
                            }
                        }

                        if (!isExists)
                        {
                            var fileInfo = new FileInfo(file);

                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                            }
                        }
                    }
                }
            }

            

            
        }








        private void RenameDirectory(string oldName,string newName)
        {
            var oldPath = $"{Directory.GetCurrentDirectory()}/wwwroot/Files/{oldName}";

            var newPath = $"{Directory.GetCurrentDirectory()}/wwwroot/Files/{newName}";


            if (Directory.Exists(oldPath)&&!Directory.Exists(newPath)) 
            {
                Directory.Move(oldPath, newPath);
            }

        }



        private async Task LoadImages(HttpRequest request, Room? room, ApplicationContext context)
        {
            var files = request.Form.Files;

            var uploadPath = $"{Directory.GetCurrentDirectory()}/wwwroot/Files/{room.Name}";




            var Logo = request.Form["LogoPath"];


            


            for (int i = 0; i < files.Count; i++)
            {
                string Image = $"{files[i].FileName}";

                //Если true значит было загружено Лого и копировать файлы нужно со 2-го

                if (i == 0&&Logo=="")
                {
                    //Load Logo
                    room.LogoPath = Image;
                }
                else
                {
                    //Load Image
                    context.Images.Add(new() { Room = room, Path = Image });
                }

                

                string ImagePath = $"{uploadPath}/{files[i].FileName}";


                using (var fs = new FileStream(ImagePath, FileMode.Create))
                {
                    await files[i].CopyToAsync(fs);
                }
            }
        }


        private void UpdateImages()
        {
            context.RemoveRange(context.Images.Where(i => i.RoomId == Room.Id));

            var images = Request.Form["ImagePath"];

            if (!images.IsNullOrEmpty())
            {
                foreach (var image in images)
                {
                    context.Images.Add(new() { Path= image,Room=Room });
                }
            }
            
        }



        private void UpdatePhones()
        {
            

            foreach (var phone in Request.Form["Phone"])
            {
                if(phone!="")
                context.Phones.Add(new() { Number = phone, Room = Room });
            }
        }


        private void RemovePhones()
        {
            var phones = context.Phones.Where(p => p.RoomId == Room.Id);

            if (phones != null)
            {
                context.Phones.RemoveRange(phones);
            }
        }


        private async Task UpdateLogo()
        {
            var Logo = Request.Form["LogoPath"];

            if (Logo.IsNullOrEmpty())
            {
                await LoadLogo(Request, Room, context);
            }
            else
            {
                //Если старое лого было удалено
                if (Logo == "") { await LoadLogo(Request, Room, context); }
                else { Room.LogoPath = Logo; }
            }
        }



        public static async Task LoadLogo(HttpRequest request, Room? room, ApplicationContext context)
        {
            var files = request.Form.Files;

            

            var uploadPath = $"{Directory.GetCurrentDirectory()}/wwwroot/Files/{room.Name}";
            Directory.CreateDirectory(uploadPath);



            room.LogoPath = $"{room.Name}/{files[0].FileName}";


            string LogoPath = $"{uploadPath}/{files[0].FileName}";


            using (var fs = new FileStream(LogoPath, FileMode.Create))
            {
                await files[0].CopyToAsync(fs);
            }

        }



        public static async Task LoadFiles(HttpRequest request, Room? room, ApplicationContext context)
        {
            var files = request.Form.Files;



            var uploadPath = $"{Directory.GetCurrentDirectory()}/wwwroot/Files/{room.Name}";
            Directory.CreateDirectory(uploadPath);




            room.LogoPath = $"{files[0].FileName}";


            string LogoPath = $"{uploadPath}/{files[0].FileName}";


            using (var fs = new FileStream(LogoPath, FileMode.Create))
            {
                await files[0].CopyToAsync(fs);
            }


            

            for (int i = 1; i < files.Count; i++)
            {
                string Image = $"{files[i].FileName}";

                context.Images.Add(new() { Room = room, Path = Image });

                string ImagePath = $"{uploadPath}/{files[i].FileName}";


                using (var fs = new FileStream(ImagePath, FileMode.Create))
                {
                    await files[i].CopyToAsync(fs);
                }
            }






        }


        public static async Task DeleteFiles(string roomName)
        {
            var deletePath = $"{Directory.GetCurrentDirectory()}/wwwroot/Files/{roomName}";

            if (Directory.Exists(deletePath))
            {
                Directory.Delete(deletePath, true);
            }
        }
    }
}
