using MyWebApp1.Data;
using MyWebApp1.Entities;
using MyWebApp1.Models;

namespace MyWebApp1.Services
{
    public class PetService
    {
        private readonly MyDbContext _context;

        public PetService(MyDbContext context)
        {
            _context = context;
        }

        public Entities.Pet AddNewPet(Models.PetDTO petDTO)
        {
            var pet = new Entities.Pet
            {
                PetName = petDTO.PetName,
                PetType = petDTO.PetType,
                Age = petDTO.Age,
                Gender = petDTO.Gender,
                Address = petDTO.Address,
                MedicalCondition = petDTO.MedicalCondition,
                ContactPhoneNumber = petDTO.ContactPhoneNumber,
                ContactEmail = petDTO.ContactEmail
            };

            _context.Pets.Add(pet);
            _context.SaveChanges();
            return pet;
        }
    }


}
