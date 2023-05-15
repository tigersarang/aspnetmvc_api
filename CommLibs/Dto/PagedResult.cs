using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommLibs.Dto
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } // 현재 페이지의 아이템들
        public int TotalCount { get; set; } // 전체 아이템 개수

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); // 전체 페이지 개수
        public int PageNumber { get; set; } // 현재 페이지
        public int PageSize { get; set; } = 10; // 페이지당 아이템 개수

        public string Search { get; set; } // 검색어

        public int StartPage 
        { get
            {
                return (PageNumber - 1) / PageSize * PageSize + 1;
            }
        } // 페이지네이션 시작 페이지
        public int EndPage { get
            {
                if (StartPage + 9 < TotalPages)
                {
                    return StartPage + 9;
                } else
                {
                    return TotalPages;
                }
            }
        } // 페이지네이션 끝 페이지
    }
}
