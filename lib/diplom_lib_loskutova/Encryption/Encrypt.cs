namespace diplom_lib_loskutova.Encryption
{
    public class Encrypt
    {
        public string encrypt(string inputtext)
        {
            string key = "13371337", k2 = "", lpst_c = "", st_k = "";
            string Alphabet = "?><./,:';|{}[]+_=-()*&^%$#@!0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZab" +
                "cdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЫЭЮЯабвгдеёжзийклмнопрстуфхцчшщъьыэюя ";
            int j = 0, ki = 0, ki_k1 = 0;

            for (int i = 0; i <= inputtext.Length - 1; i++)
            {
                k2 += key[j];
                j++;
                if (j > key.Length - 1)
                    j = 0;
            }

            for (int i = 0; i <= inputtext.Length - 1; i++)
            {
                for (int k1Index = 0; k1Index <= Alphabet.Length - 1; k1Index++)
                {
                    if (inputtext[i] == Alphabet[k1Index])
                    {
                        st_k = k2[i].ToString();
                        ki = int.Parse(st_k);
                        ki_k1 = k1Index + ki;

                        if (ki_k1 > Alphabet.Length - 1)
                        {
                            ki_k1 = (ki_k1 - Alphabet.Length);
                        }

                        st_k = Alphabet[ki_k1].ToString();
                        lpst_c += st_k;
                    }
                }
            }
            return lpst_c;
        }
    }
}
