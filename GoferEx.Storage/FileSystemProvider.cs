using System;
using System.Collections.Generic;
using System.IO;
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

        public bool AddContacts(List<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                var contactPath = Path.Combine(_baseContactDir, contact.Id.ToString());
                File.Create(contactPath).Close();
                File.WriteAllText(contactPath, contact.ToString());
            }
            return true;
        }

        public Contact GetContact(Guid id)
        {
            var contactPath = Path.Combine(_baseContactDir, id.ToString());
            if (File.Exists(contactPath))
            {
                return JsonConvert.DeserializeObject<Contact>(File.ReadAllText(contactPath));
            }
            else
            {
                return null;
                //throw new FileNotFoundException(contactPath);
            }
        }

        public List<Contact> GetContacts()
        {
            List<Contact> contacts = new List<Contact>();
            foreach (var filePath in Directory.EnumerateFiles(_baseContactDir))
            {
                contacts.Add(JsonConvert.DeserializeObject<Contact>(File.ReadAllText(filePath)));
            }

            return contacts;
        }

        public bool RemoveContact(Guid id)
        {
            File.Delete(Path.Combine(_baseContactDir, id.ToString()));
            return true;
        }

        public bool UpdateContacts(List<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                var contactPath = Path.Combine(_baseContactDir, contact.Id.ToString());
                if (File.Exists(contactPath))
                {
                    File.WriteAllText(contactPath, contact.ToString());
                }
            }

            return true;
        }
    }
}
