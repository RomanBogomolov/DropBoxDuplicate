using System;
using System.Security.Cryptography;

namespace DropBoxDuplicate.Api.Services

{
    /// <summary>
    /// Этот класс может генерировать случайные пароли, которые не содержат неоднозначных
    /// символов, таких как I, I и 1. Сгенерированный пароль будет состоять из
    /// 7-битных символы ASCII. Каждые четыре символа будут содержать один нижний регистр,
    /// один символ верхнего регистра, один номер и один специальный символ
    /// (например, '%') в случайном порядке. Пароль всегда начинается с
    /// буквенно-цифровой символа; Он не будет начинаться со специального символа (это делается
    /// потому, что некоторые серверные системы не любят некоторые специальные
    /// символов в первой позиции).
    /// </summary>
    public class RandomPasswordService
    {
        /*
         * Минимальная и максимальная длина пароля
         */
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 6;

        private static int DEFAULT_MAX_PASSWORD_LENGTH = 8;

        /* 
         * Опредление поддерживаемых символов пароля. Можно добавлять / удалять символы из групп 
         */
        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";

        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";

        private static string PASSWORD_CHARS_NUMERIC = "23456789";
        //private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

        /// <summary>
        /// Генерация рандомного пароля
        /// </summary>
        /// <returns>
        /// Сгенерированный пароль
        /// </returns>
        /// <remarks>
        /// Длина сгенерированного пароля будет определена при Random()
        /// Он не будет корочя минимального заданного и длиннне максимального
        /// </remarks>
        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                DEFAULT_MAX_PASSWORD_LENGTH);
        }

        /// <summary>
        /// Генерирует случайный пароль точной длины.
        /// </summary>
        /// <param name="length">
        /// Точная длина пароля.
        /// </param>
        /// <returns>
        /// Сгенерированный пароль
        /// </returns>
        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        /// <summary>
        /// Генерация рандомного пароля
        /// </summary>
        /// <param name="minLength">
        /// Минимальная длина пароля
        /// </param>
        /// <param name="maxLength">
        /// максимальная длина пароля
        /// </param>
        /// <returns>
        /// Рандомно сгенерированный пароль
        /// </returns>
        /// <remarks>
        /// Длина сгенерированного пароля будет определена при
        /// random и он будет падать с диапазоном, определяемым
        /// функцией.
        /// </remarks>
        public static string Generate(int minLength,
            int maxLength)
        {
            /*
             * Убеждаемся в валидности входных параметров
             */
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Создаем локальный массив, содержащий поддерживаемые символы пароля
            // сгруппированы по типам. Можно удалить группы из этого списка
            // массива, но это ослабит силу пароля.
            char[][] charGroups =
            {
                PASSWORD_CHARS_LCASE.ToCharArray(),
                PASSWORD_CHARS_UCASE.ToCharArray(),
                PASSWORD_CHARS_NUMERIC.ToCharArray()
                //,PASSWORD_CHARS_SPECIAL.ToCharArray()
            };

            // Использовать этот массив для отслеживания количества неиспользованных символов в каждой
            // группе символов.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Изначально все символы в каждой группе не используются.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Использовать этот массив для отслеживания (перебора) неиспользуемых групп символов.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Изначально все группы символов не используются.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Поскольку мы не можем использовать рандомизатор по умолчанию, который основан на
            // текущее время (оно будет производить тот же «случайный» номер в пределах
            // секумны), мы будем использовать генератор случайных чисел для заполнения
            // randomizer.

            // Использовать 4-байтный массив, чтобы заполнить его случайными байтами и преобразовать его затем
            // до целочисленного значения.
            byte[] randomBytes = new byte[4];

            // Генерация рандомных 4 байт
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Конвертация 4 байт в 32 бита типа integer
            int seed = BitConverter.ToInt32(randomBytes, 0);

            // Теперь это реальная рандомизация
            Random random = new Random(seed);

            // Этот массив будет содержать символы пароля.
            char[] password;

            // Выделяем подходящую память для пароля.
            password = minLength < maxLength ? new char[random.Next(minLength, maxLength + 1)] : new char[minLength];

            // Индекс следующего символа, который будет добавлен в пароль.
            int nextCharIdx;

            // Индекс следующей группы символов, которая будет обработана.
            int nextGroupIdx;

            // Индекс, который будет использоваться для отслеживания не обработанных групп символов.
            int nextLeftGroupsOrderIdx;

            // Индекс последнего необработанного символа в группе
            int lastCharIdx;

            // Индекс последней необработанной группы.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Генерируем символы пароля по одному за раз.
            for (int i = 0; i < password.Length; i++)
            {
                // Если только одна группа символов осталась необработанной, обработаем ее;
                // в противном случае выбираем произвольную группу символов из необработаннго
                // списка групп. Чтобы разрешить появление специального символа в
                // первой позиция, увеличиваем второй параметр Next
                // единождым вызовов функции, то есть lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                        lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Получить фактический индекс группы символов, из которого мы будем
                // выбирать следующий символ.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // Если остался только один необработанный символ, выберем его; в противном случае,
                // получить случайный символ из списка неиспользуемых символов.
                nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);

                // Добавить этот символ в пароль.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // Если мы обработали последний символ в этой группе, начнем сначала.
                if (lastCharIdx == 0)
                {
                    charsLeftInGroup[nextGroupIdx] =
                        charGroups[nextGroupIdx].Length;
                }

                // Осталось много необработанных символов.
                else
                {
                    // Swap обработанный символ с последним необработанным символом
                    // чтобы мы не выбирали его, пока не обработаем все символы в
                    // этой группе.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                            charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }

                    // Уменьшение количества необработанных символов в
                    // эта группа.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // Если мы обработали последнюю группу, начнем все сначала.
                if (lastLeftGroupsOrderIdx == 0)
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }

                // Осталось много необработанных групп.
                else
                {
                    // Swap обработанной группы с последней необработанной группой
                    // чтобы не выбирать его, пока не обработаем все группы.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                            leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Уменьшение количества необработанных групп.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Преобразование символов пароля в строку и возврат результата.
            return new string(password);
        }
    }
}