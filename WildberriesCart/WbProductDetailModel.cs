namespace WildberriesCart;

public class WbProductDetailModel
{
    public int state { get; set; }
    public int payloadVersion { get; set; }
    public Data data { get; set; }

    public class Data
    {
        public List<Product> products { get; set; }
    }

    public class Product
    {
        public long id { get; set; }
        public long root { get; set; }
        public int kindId { get; set; }
        public string brand { get; set; }
        public long brandId { get; set; }
        public int siteBrandId { get; set; }
        public List<Color> colors { get; set; }
        public int subjectId { get; set; }
        public int subjectParentId { get; set; }
        public string name { get; set; }
        public string supplier { get; set; }
        public long supplierId { get; set; }
        public double supplierRating { get; set; }
        public int supplierFlags { get; set; }
        public int pics { get; set; }
        public double rating { get; set; }
        public double reviewRating { get; set; }
        public int feedbacks { get; set; }
        public int volume { get; set; }
        public int viewFlags { get; set; }
        public List<long> promotions { get; set; }
        public List<Size> sizes { get; set; }
    }

    public class Color
    {
        public string name { get; set; }
        public long id { get; set; }
    }

    public class Size
    {
        public string name { get; set; }
        public string origName { get; set; }
        public int rank { get; set; }
        public long optionId { get; set; }
        public List<Stock> stocks { get; set; }
    }

    public class Stock
    {
    }
}