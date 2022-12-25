using System.Security.Cryptography;
using System.Text;

namespace MerkleTree {
    class Program {
        static void Main(string[] args) {
            var transactions = new List<string>() {"aaa", "bbb", "ccc", "ddd", "eee"};

            var transactionHashes = new List<string>();

            transactionHashes = GetTransactionHashes(transactions);

            string merkleRoot = ComputeMerkleRoot(transactionHashes);

            //bitcoin display hash in little endian format
            Console.WriteLine("Computed Merkle Root: " + SwapAndReverse(merkleRoot).ToLower());
        }

       private static  string ComputeMerkleRoot(IList<string> merkleLeaves) {
            if(merkleLeaves == null || !merkleLeaves.Any())

                return string.Empty;

            if(merkleLeaves.Count() == 1) {
                return merkleLeaves.First();
            }

            if(merkleLeaves.Count() % 2 > 0) {
                merkleLeaves.Add(merkleLeaves.Last());
            }

            var merkleBranches = new List<string>();

            for(int i = 0; i < merkleLeaves.Count(); i += 2) {
                var leafPair= string.Concat(merkleLeaves[i], merkleLeaves[i + 1]);
                merkleBranches.Add(ComputeDoubleHash(leafPair));
            }
            return ComputeMerkleRoot(merkleBranches);
        }

        private static string ComputeSHA256(string data, bool initial = false) {
            using(var sha256 = SHA256.Create()) {
                if(initial) {
                    return ComputeHash(sha256, Encoding.UTF8.GetBytes(data));
                }
                else {
                    return ComputeHash(sha256, HexToByteArray(data));
                }
            }
        }

        private static string ComputeDoubleHash(string data, bool initial = false) {
            return ComputeSHA256(ComputeSHA256(data, initial), initial);
        }

        private static string ComputeHash(HashAlgorithm hashAlgorithm, byte[] input) {
            byte[] data = hashAlgorithm.ComputeHash(input);
            return ByteArrayToHex(data);
        }

        private static string ByteArrayToHex(byte[] bytes) {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        private static byte[] HexToByteArray (string hex) {
            return Enumerable.Range (0, hex.Length)
                .Where (x => x % 2 == 0)
                .Select (x => Convert.ToByte (hex.Substring (x, 2), 16))
                .ToArray ();
        }

        // for little endian bitcoin protocol
        private static string SwapAndReverse(string input) { 
            string newString = string.Empty;;
            for(int i = 0; i < input.Count(); i += 2) {
                newString += string.Concat(input[i + 1], input[i]);
            }
            return ReverseString(newString);
        }

        private static string ReverseString(string original) {
            return new string(original.Reverse().ToArray());
        }

        private static List<string> GetTransactionHashes(List<string> transactions)
        {

            var transactionHashes = new List<string>();

            foreach(var transaction in transactions) {
                transactionHashes.Add(ComputeDoubleHash(transaction, true));
            }

            return transactionHashes;
        }
    }
}