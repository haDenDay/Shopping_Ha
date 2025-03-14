namespace Shopping_Ha.Models
{
    public class Paginate
    {
        public int ToTalItems { get; set; }//Tỏng số item
        public int PageSize { get; set; }//Tổng Item/Trag
        public int CurrentPage { get; set; }//Trang hiện tại
        public int TotalPages { get; set; }//Tổng trang
        public int StartPage { get; set; }//Trag bắt đầu
        public int EndPage { get; set; }//Trag kết thúc
        public Paginate()
        {

        }
        public Paginate(int totalItems, int page, int pageSize = 10)//10 item trên 1 trang  
        {
            //Làm tròn tổng item/10 items trên 1 trang 
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);//33/3 = 3.3 ceiling làm tròn lên 4 trag

            int currentPage = page; //Page hiện tại là 1

            int startPage = currentPage - 5;//Trag bắt đầu trừ đi 5 button
            int endPage = currentPage + 4;//Trang cuối sẽ cộng thêm 4 button

            if (startPage <= 0)
            {
                //nếu số trang bắt đầu nhỏ hơn or = 0 thì số trang cuối sẽ bằng
                endPage = endPage-(startPage-1);//6-(-3-1) =10
                startPage = 1;
            }
            if(endPage > totalPages)//nếu số page cuối > số tổng trang
            {
                endPage = totalPages;//số page cuối = số tổng trang
                if(startPage > 10)//nếu số page cuối >10    
                {
                    startPage = endPage - 9;//trag bắt đầu = trag cuối  -9
                }
            }

            ToTalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
