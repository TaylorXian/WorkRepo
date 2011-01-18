using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace CommonCS
{
    /// <summary>
    /// Summary description for Token.
    /// </summary>
    public class Token
    {
        public Token() {}

        public static void SetConfigFile(string sCfgFile)
        {
            _sCfgFile = sCfgFile;
        }

        private static string _GetConfigFilePath()
        {
            if (null != System.Web.HttpContext.Current)
            {
                return System.Web.HttpContext.Current.Server.MapPath("~/Config/token.xml");
            }
            else
            {
                Debug.Assert(null != _sCfgFile);
                return _sCfgFile;
            }
        }

        private static void _LoadFromFile()
        {
            string sServerCfgPath = _GetConfigFilePath();

            StreamReader fs = new StreamReader(sServerCfgPath);
            XmlReader reader = new XmlTextReader(fs);
            XmlSerializer serializer = new XmlSerializer(typeof(Token));
            _svr = (Token)serializer.Deserialize(reader);

            fs.Close();
        }

        public static Token GetToken()
        {
            if (null == _svr)
            {
                _LoadFromFile();
            }

            return _svr;
        }

        private static Token _svr = null;

        [XmlElement("HomePage")]
        public string _sHomePage;

        [XmlElement("Login-Register")]
        public string _sLogin_Register;

        [XmlElement("Department")]
        public string _sDepartment;

        [XmlElement("MerchantDetail")]
        public string _sMerchantDetail;

        [XmlElement("CompPriceComparison")]
        public string _sCompPriceComparison;

        [XmlElement("BrowseHistory")]
        public string _sBrowseHistory;

        [XmlElement("SpringFestival")]
        public string _sSpringFestival;

        [XmlElement("SiteMap")]
        public string _sSitemap;

        [XmlElement("MerchantService")]
        public string _sMerchantService;

        [XmlElement("AboutUs")]
        public string _sAboutUs;

        [XmlElement("ContactUs")]
        public string _sContactUs;

        [XmlElement("DevelopmentCourse")]
        public string _sDevelopmentCourse;

        [XmlElement("Disclaimer")]
        public string _sDisclaimer;

        [XmlElement("Help")]
        public string _sHelp;

        [XmlElement("ServiceAgreement")]
        public string _sServiceAgreement;

        [XmlElement("Recruit")]
        public string _sRecruit;

        [XmlElement("FeedBack")]
        public string _sFeedback;

        [XmlElement("Introduction")]
        public string _sIntroduction;

        [XmlElement("Registration")]
        public string _sRegistration;

        [XmlElement("Activation")]
        public string _sActivation;

        [XmlElement("ChangeEmail")]
        public string _sChangeEmail;

        [XmlElement("ChangeOtherInfo")]
        public string _sChangeOtherInfo;

        [XmlElement("ChangePassword")]
        public string _sChangePassword;

        [XmlElement("ForgotPassword")]
        public string _sForgotPassword;

        [XmlElement("RegistrationThankyou")]
        public string _sRegistrationThankyou;

        [XmlElement("RegistrationThankyouWithReview")]
        public string _sRegistrationThankyouWithReview;

        [XmlElement("SendPassword")]
        public string _sSendPassword;

        [XmlElement("UserInfo")]
        public string _sUserInfo;

        [XmlElement("ValidateCode")]
        public string _sValidateCode;

        [XmlElement("Article")]
        public string _sArticle;

        [XmlElement("CompArticleList")]
        public string _sCompArticleList;

        [XmlElement("Adult")]
        public string _sAdult;

        [XmlElement("BookAndMusic")]
        public string _sBookAndMusic;

        [XmlElement("CellPhone")]
        public string _sCellPhone;

        [XmlElement("Cosmetic")]
        public string _sCosmetic;

        [XmlElement("Digital")]
        public string _sDigital;

        [XmlElement("Infant")]
        public string _sInfant;

        [XmlElement("Sport")]
        public string _sSport;

        [XmlElement("Auto")]
        public string _sAuto;

        [XmlElement("Cars")]
        public string _sCars;

        [XmlElement("House")]
        public string _sHouse;

        [XmlElement("Dress")]
        public string _sDress;

        [XmlElement("DeliveryInfo")]
        public string _sDeliveryInfo;

        [XmlElement("MerchantReview")]
        public string _sMerchantReview;

        [XmlElement("MerchantReviewPreview")]
        public string _sMerchantReviewPreview;

        [XmlElement("MerchantReviewSubmit")]
        public string _sMerchantReviewSubmit;

        [XmlElement("Shop")]
        public string _sShop;

        [XmlElement("CompProductList")]
        public string _sCompProdcutList;

        [XmlElement("CompSearchResult")]
        public string _sCompSearchResult;

        [XmlElement("NCProductList")]
        public string _sNCProductList;

        [XmlElement("NCSearchResult")]
        public string _sNCSearchResult;

        [XmlElement("ProductDetail")]
        public string _sProductDetail;

        [XmlElement("NCProductDetail")]
        public string _sNCProductDetail;

        [XmlElement("ProductImageShow")]
        public string _sProductImageShow;

        [XmlElement("CompareProducts")]
        public string _sCompareProducts;

        [XmlElement("ProductAttributes")]
        public string _sProductAttributes;

        [XmlElement("MoreAttributes")]
        public string _sMoreAttributes;

        [XmlElement("ProductReview")]
        public string _sProductReview;

        [XmlElement("PreviewProductReview")]
        public string _sPreviewProductReview;

        [XmlElement("ProductReviewSubmit")]
        public string _sProductReviewSubmit;

        [XmlElement("NCPrice")]
        public string _sNCPrice;

        [XmlElement("BrowseHistorySwitch")]
        public string _sBrowseHistorySwitch;

        [XmlElement("CompDiffCatPros")]
        public string _sCompDiffCatPros;

        [XmlElement("Favourite")]
        public string _sFavourite;

        [XmlElement("EmailToFriend")]
        public string _sEmailToFriend;

        [XmlElement("ValentineDay")]
        public string _sValentineDay;

        [XmlElement("Women")]
        public string _sWomen;

        [XmlElement("Mother")]
        public string _sMother;

        [XmlElement("Children")]
        public string _sChildren;

        [XmlElement("Transformers")]
        public string _sTransformers;

        [XmlElement("NationalDay")]
        public string _sNationalDay;

        [XmlElement("Christmas")]
        public string _sChristmas;

        [XmlElement("MidAutumn")]
        public string _sMidAutumn;

        [XmlElement("SitemapAttributes")]
        public string _sSitemapAttributes;

        [XmlElement("SitemapCategoryList")]
        public string _sSitemapCategoryList;

        [XmlElement("TopSearch")]
        public string _sTopSearch;

        [XmlElement("DepartmentList")]
        public string _sDepartmentList;

        [XmlElement("NoCompareProduct")]
        public string _sNoCompareProduct;

        [XmlElement("NoAttribute")]
        public string _sNoAttribute;

        [XmlElement("SearchNoResult")]
        public string _sSearchNoResult;

        [XmlElement("GotoMerchantShop")]
        public string _sGoToMerchantShop;

        [XmlElement("MoreSearchResultCategories")]
        public string _sMoreSearchResultCategories;

        private static string _sCfgFile = null;
    }
}
