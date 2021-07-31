namespace Electro.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GioHang")]
    public partial class GioHang
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaKH { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaSP { get; set; }

        public int? SoLuong { get; set; }

        public virtual KhachHang KhachHang { get; set; }

        public virtual SanPham SanPham { get; set; }
        public GioHang()
        {
        }
        public GioHang(int MaSP, int SoLuong)
        {
            this.MaSP = MaSP;
            this.SoLuong = 1;
        }
        public GioHang(int MaSP, int MaKH, int SoLuong)
        {
            this.MaKH = MaKH;
            this.MaSP = MaSP;
            this.SoLuong = SoLuong;
        }
    }
}
