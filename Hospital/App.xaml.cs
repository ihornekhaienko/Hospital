using Hospital.Data;
using Hospital.Services.Implementations;
using Hospital.Services.Interfaces;
using System;
using System.Windows;

namespace Hospital
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Db db;
        internal static Random random;
        internal static IAdminService adminService;
        internal static IAddressService addressService;
        internal static IDoctorService doctorService;
        internal static IEmailService emailService;
        internal static IFacultyService facultyService;
        internal static IFileManager fileManager;
        internal static IGroupService groupService;
        internal static IRecordService recordService;
        internal static IScheduler scheduler;
        internal static ISpecialtyService specialtyService;
        internal static IStudentService studentService;
        internal static IUserManager userManager;
        public delegate void Refresh(object source);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            db = new Db();

            random = new Random();
            emailService = new EmailService();
            addressService = new AddressService(db);
            doctorService = new DoctorService(db);
            userManager = new UserManager(db);
            adminService = new AdminService(db);
            facultyService = new FacultyService(db);
            fileManager = new FileManager();
            groupService = new GroupService(db);
            recordService = new RecordService(db);
            specialtyService = new SpecialtyService(db);
            studentService = new StudentService(db);
            scheduler = new Scheduler(db);
        }
    }
}
