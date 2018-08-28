using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GoferEx.Core;
using Newtonsoft.Json;

namespace GoferEx.Storage
{
    public class FileSystemProvider : IDbProvider
    {
        private string _baseContactDir;
        private string _photoContactDir;

        public FileSystemProvider(string baseContactDir, string photoContactDir)
        {
            _baseContactDir = baseContactDir;
            _photoContactDir = photoContactDir;
            Directory.CreateDirectory(_baseContactDir);
            Directory.CreateDirectory(_photoContactDir);
        }

        public async Task<bool> AddContacts(List<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                var contactPath = Path.Combine(_baseContactDir, contact.Id.ToString());
                await Task.Run(() =>
                {
                    File.Create(contactPath).Close();
                    File.WriteAllText(contactPath, contact.ToString());
                    if (contact.Photo != null)
                    {
                        SaveImage(contact);
                    }                    
                });
            }
            return true;
        }

        public async Task<Contact> GetContact(Guid id)
        {
            var contactPath = Path.Combine(_baseContactDir, id.ToString());
            if (File.Exists(contactPath))
            {
                var contact = JsonConvert.DeserializeObject<Contact>(File.ReadAllText(contactPath));
                contact.Photo = GetImage(contact);
                return JsonConvert.DeserializeObject<Contact>(File.ReadAllText(contactPath));
            }
            else
            {
                return null;                
            }
        }

        public async Task<IEnumerable<Contact>> GetContacts()
        {
            List<Contact> contacts = new List<Contact>();
            foreach (var filePath in Directory.EnumerateFiles(_baseContactDir))
            {
                var contact = JsonConvert.DeserializeObject<Contact>(File.ReadAllText(filePath));
                contact.Photo = GetImage(contact);
                contacts.Add(contact);
            }

            if (contacts.Count == 0)
            {
                return null;
            }
            return contacts;
        }

        public async Task<bool> RemoveContact(Guid id)
        {
            await Task.Run(()=>File.Delete(Path.Combine(_baseContactDir, id.ToString())));
            return true;
        }

        public async Task<bool> UpdateContacts(List<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                var contactPath = Path.Combine(_baseContactDir, contact.Id.ToString());
                if (File.Exists(contactPath))
                {
                    await Task.Run(() => File.WriteAllText(contactPath, contact.ToString()));
                }
            }

            return true;
        }

        private void SaveImage(Contact contact)
        {
            var imgContactPath = Path.Combine(_photoContactDir, contact.Id.ToString());
            using (var fs = File.Create(imgContactPath))
            {
                fs.Write(contact.Photo, 0, contact.Photo.Length);
            }
        }

        private byte[] GetImage(Contact contact)
        {
            var imgContactPath = Path.Combine(_photoContactDir, contact.Id.ToString());
            if (File.Exists(imgContactPath))
            {
                return File.ReadAllBytes(imgContactPath);
            }

            return null;
        }
    }
}
