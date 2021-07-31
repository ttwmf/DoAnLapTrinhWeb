use master
go
create database Electro
go
use Electro
go
create table NhaSanXuat(
	MaNSX int identity (1,1) primary key,
	TenNSX nvarchar(200),
	ThongTin nvarchar(4000)
)

create table NhaCungCap (
	MaNCC int identity (1,1) primary key,
	TenNCC nvarchar(200),
	DiaChi nvarchar(200),
	Email varchar(200),
	DienThoai varchar(10)
)

create table SanPham (
	MaSP int identity (1,1) primary key,
	MaNCC int references NhaCungCap(maNCC),
	MaNSX int references NhaSanXuat(MaNSX),	
	TenSP nvarchar(200) not null,
	DonGia money,
	SoLuongTon int check (SoLuongTon >= 0),
	NgayCapNhat Date,
	CauHinh nvarchar(4000),
	MoTa nvarchar(4000),
	HinhAnh nvarchar(500),
	LuotXem int,
	TrungBinhDanhGia float,
	DaXoa bit default ('false')
)
go

create table KhachHang (
	MaKH int identity(1,1) primary key,
	HoTen nvarchar(200) not null,
	TaiKhoan varchar(200) not null,
	MatKhau varchar(200) not null,
	DiaChi nvarchar (200),
	Email varchar(200),
	DienThoai varchar(10),
)


create table BinhLuan (
	MaBinhLuan int identity (1,1) primary key,
	NguoiBinhLuan nvarchar(200),
	Email varchar(200),
	NoiDung nvarchar(4000) not null,
	MaSP int references SanPham(MaSP),
	DanhGia int,
	NgayBinhLuan date default GetDate()
)
go
create table PhieuNhap (
	MaPN int identity(1,1) primary key,
	MaNCC int references NhaCungCap(MaNCC),
	NgayNhap Date,
	DaXoa bit
)
go
create table ChiTietPhieuNhap(
	MaPN int references PhieuNhap(MaPN),
	MaSP int references SanPham(MaSP),
	TenSP nvarchar(200),
	DonGiaNhap money not null,
	SoLuongNhap int constraint CHK_SL check (SoLuongNhap > 0),
	Constraint PK_CTPN primary key(MaPN, MaSP)
)
go
create table DonDatHang (
	MaDDH int identity (1,1) primary key,
	NgayLap Date,
	MaKH int null,
	Hoten nvarchar(400),
	DiaChiGiaoHang nvarchar(400),
	DienThoai varchar(10),
	Email varchar(200),
	PhuongThucThanhToan nvarchar(200),
	TongTien money
	foreign key (MaKH) references KhachHang(MaKH),
)
go
create table ChiTietDonDatHang (
	MaDDH int references DonDatHang(MaDDH),
	MaSP int references SanPham(MaSP),
	TenSP nvarchar(200),
	SoLuong int check (SoLuong > 0),
	DonGiaBan money,
	Constraint PK_CTDDH primary key(MaDDH, MaSP)
)
go

create table GioHang(
	MaKH int references KhachHang(MaKH),
	MaSP int references SanPham(MaSP),
	SoLuong int,
	Constraint PK_CTGH primary key(MaKH, MaSP)
)
go
create table KhuyenMai (
	MaKM int identity(1,1) primary key,
	TenChuongTrinh nvarchar(4000) not null,
	NoiDung nvarchar(4000),
	NgayBatDau date,
	NgayKetThuc date,
)
go
create table ChiTietKhuyenMai(
	MaKM int references KhuyenMai(MaKM),
	MaSP int references SanPham(MaSP),
	GiamGia int
	Constraint PK_CTKM primary key(MaKM, MaSP)
)
go
create table Admin (
	MaAdmin int identity(1,1) primary key,
	HoTen nvarchar(200) not null,
	TaiKhoan varchar(200),
	matkhau varchar(200),
	DiaChi Nvarchar(500),
	Email varchar(200),
	DienThoai varchar(10),
	NgaySinh Date
)
go

go
DROP TABLE BinhLuan
DROP TABLE ChiTietDonDatHang
DROP TABLE DonDatHang
DROP TABLE GioHang
DROP TABLE KhachHang
DROP TABLE ChiTietKhuyenMai
DROP TABLE KhuyenMai
DROP TABLE ChiTietPhieuNhap
DROP TABLE PhieuNhap
DROP TABLE SanPham
DROP TABLE NhaCungCap
DROP TABLE NhaSanXuat
DROP TABLE Admin