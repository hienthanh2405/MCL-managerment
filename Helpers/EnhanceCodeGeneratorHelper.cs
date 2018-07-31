using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class EnhanceCodeGeneratorHelper<TEntity> where TEntity : CodeBaseEntity
    {
        private DbSet<TEntity> _dbset;
        private DatabaseContext _context;

        public EnhanceCodeGeneratorHelper(DatabaseContext context)
        {
            _context = context;
            _dbset = _context.Set<TEntity>();

        }
        private string _getGeneratedCode(string prefix, string lastestCode, int wantedNumberLength)
        {
            DateTime today = DateTime.Now;
            string year = today.Year.ToString();
            string dateString = year;

            if (lastestCode == null)
            {
                return prefix + dateString + "1".PadLeft(wantedNumberLength, '0');
            }

            int number = Int32.Parse(lastestCode.Substring(lastestCode.Length - wantedNumberLength));

            string dateOflastestCode = lastestCode.Substring(prefix.Length ,  4 );
            // ex: lastestCode PX2018000001 => dateOflastestCode = 2018

            if (dateOflastestCode != dateString) // this is the fist code of day
            {
                number = 0;
            }
            return prefix + dateString + (number + 1).ToString().PadLeft(wantedNumberLength, '0');
        }

        private async Task<bool> _isDuplicatedCode(string code)
        {
            var warehousing = await _dbset.FirstOrDefaultAsync(w => w.Code == code);
            if (warehousing != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> ReturnCode(string prefix, int wantedNumberLength)
        {
            bool isDuplicated = false;
            string code;
            int maxLoop = 500;
            int loop = 0;
            do
            {
                
                string lastestCode = _dbset.Max(w => w.Code);
                code = _getGeneratedCode(prefix, lastestCode, wantedNumberLength);
                isDuplicated = await _isDuplicatedCode(code);
                loop++;
                if (loop  == maxLoop)
                {
                    throw new Exception("It is too long to generate code");
                }

            } while (isDuplicated);
            return code;
        }
    }
}
