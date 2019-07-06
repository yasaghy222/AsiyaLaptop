using Mapster;
using Src.Models.Data;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Src.App_Start
{
    public class MapsterConfig
    {
        public static void RegisterMap() {
            #region mapster
            #region admin
            TypeAdapterConfig<Tbl_Admin, Admin.ViewAdmin>.NewConfig()
               .Map(dest => dest.FullName, src => $"{src.Name} {src.Family}")
               .Map(dest => dest.RoleName, src => src.Tbl_ARole.ShowTitle);

            TypeAdapterConfig<Tbl_Admin, Admin.ViewTbl_Admin>.NewConfig()
                .Map(dest => dest.RoleName, src => src.Tbl_ARole.ShowTitle);

            TypeAdapterConfig<Admin.ViewTbl_Admin, Tbl_Admin>.NewConfig()
                .Map(dest => dest.RoleID, src => 1);
            #endregion

            #region product
            TypeAdapterConfig<Tbl_Product, Product.ViewProc>.NewConfig()
                   .Map(dest => dest.BrandName, src => src.Tbl_ProcBrand.Title)
                   .Map(dest => dest.CatName, src => src.Tbl_ProcCat.GetCatList())
                   .Map(dest => dest.Price, src => src.Price.SetCama())
                   .Map(dest => dest.OffPrice, src => src.OffPrice.SetCama())
                   .Map(dest => dest.Type, src => (EnumExtensions
                                                   .GetEnumValue<Product.ProcType>(src.Type))
                                                   .GetAttribute<DisplayAttribute>().Name);

            TypeAdapterConfig<Tbl_Product, Product.ViewTbl_Proc>.NewConfig()
                   .Map(dest => dest.FullDesc, src => HttpUtility.HtmlDecode(src.FullDesc));

            TypeAdapterConfig<Tbl_Product, Product.FullSearchResult>.NewConfig()
                   .Map(dest => dest.Price, src => src.Price.SetCama())
                   .Map(dest => dest.OffPrice, src => src.OffPrice.SetCama())
                   .Map(dest => dest.Type, src => (EnumExtensions
                                                   .GetEnumValue<Product.ProcType>(src.Type))
                                                   .GetAttribute<DisplayAttribute>().Name);

            TypeAdapterConfig<Tbl_Product, Product.Brand>.NewConfig()
                .Map(dest => dest.ID, src => src.BrandID)
                .Map(dest => dest.Title, src => src.Tbl_ProcBrand.Title)
                .Map(dest => dest.EnTitle, src => src.Tbl_ProcBrand.EnTitle);
            #endregion

            #region procBrand
            TypeAdapterConfig<Tbl_ProcBrand, Product.ViewTbl_ProcBrand>.NewConfig()
                   .Map(dest => dest.AssignCount, src => src.Tbl_Product.Count);
            #endregion

            #region product category
            TypeAdapterConfig<Tbl_ProcCat, Product.ViewTbl_ProcCat>.NewConfig()
                   .Map(dest => dest.AssignCount, src => src.Tbl_Product.Count());

            TypeAdapterConfig<Tbl_ProcCat, Common.FullTree>.NewConfig()
                   .Map(dest => dest.AssignCount, src => src.Tbl_Product.Count())
                   .Map(dest => dest.HasChild, src =>
                                               GenericFunction<Tbl_ProcCat>
                                               .HasChild(src, item => item.PID == src.ID));
            #endregion

            #region product property
            TypeAdapterConfig<Tbl_PCPGroup, Common.FullTree>.NewConfig()
                   .Map(dest => dest.AssignCount, src => src.Tbl_ProcProp.Count())
                   .Map(dest => dest.HasChild, src =>
                                               GenericFunction<Tbl_PCPGroup>
                                               .HasChild(src, item => item.PID == src.ID));

            TypeAdapterConfig<Tbl_PCPGroup, Product.CatProp>.NewConfig()
                   .Map(dest => dest.ValueList, src => src.Tbl_ProcProp.Where(item => item.PCPGID == src.ID).Select(item => item.Value).ToList());
            #endregion

            #region menu
            TypeAdapterConfig<Tbl_Menu, Menu.ViewTbl_Menu>.NewConfig()
                   .Map(dest => dest.HasChild, src =>
                                               GenericFunction<Tbl_Menu>
                                               .HasChild(src, item => item.PID == src.ID));
            #endregion

            #region media
            TypeAdapterConfig<Tbl_Media, Media.ViewTbl_Media>.NewConfig()
                   .Map(dest => dest.DispLoc, src => EnumExtensions.GetEnumValue<Media.MediaLocation>(src.Location));
            #endregion

            #region factor
            TypeAdapterConfig<Tbl_Factor, Factor.ViewOrder>.NewConfig()
                .Map(dest => dest.SubmitDate, src => src.SubmitDate.ToPersianDate(""))
                .Map(dest => dest.CustName, src => src.Tbl_CustAddress.Name)
                .Map(dest => dest.PaymentStatus, src => src.PaymentStatus ? Factor.PaymentStatus.Paid : Factor.PaymentStatus.UnPaid);

            TypeAdapterConfig<Tbl_Factor, Factor.ViewOrderDetail>.NewConfig()
                   .Map(dest => dest.CustName, src => src.Tbl_Customer.Name)
                   .Map(dest => dest.CustAddress, src => src.Tbl_CustAddress)
                   .Map(dest => dest.OrderProc, src => src.Tbl_FactProc)
                   .Map(dest => dest.PaymentStatus, src => src.PaymentStatus ?
                                                           Factor.PaymentStatus.Paid : Factor.PaymentStatus.UnPaid)
                   .Map(dest => dest.TotalPrice, src => src.TotalPrice.SetCama())
                   .Map(dest => dest.Status, src => (EnumExtensions
                                                     .GetEnumValue<Factor.FactStatus>(src.Status))
                                                     .GetAttribute<DisplayAttribute>().Name)
                   .Map(dest => dest.SubmitDate, src => src.SubmitDate.ToPersianDate("default"));
            #endregion
            #endregion
        }
    }
}