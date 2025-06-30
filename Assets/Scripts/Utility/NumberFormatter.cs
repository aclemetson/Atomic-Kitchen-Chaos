

namespace AtomicKitchenChaos.Utility {
    public static class NumberFormatter {
        private static readonly string[] suffixes = { "", "k", "M", "B", "T", "q", "Q", "A", "B", "C", "D" };

        public static string FormatNumber(long number) {
            if (number < 1000)
                return number.ToString();

            double value = number;
            int suffixIndex = 0;

            while (value >= 1000 && suffixIndex < suffixes.Length - 1) {
                value /= 1000;
                suffixIndex++;
            }

            return value.ToString("0.00") + suffixes[suffixIndex];
        }
    }
}