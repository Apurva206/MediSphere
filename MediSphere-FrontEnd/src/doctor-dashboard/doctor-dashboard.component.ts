import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient , HttpHeaders} from '@angular/common/http';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faAddressCard, faCalendarCheck, faNotesMedical, faFile, faRightFromBracket } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-doctor-dashboard',
  imports: [RouterLink,RouterLinkActive,RouterOutlet,CommonModule,FormsModule,FontAwesomeModule],
  templateUrl: './doctor-dashboard.component.html',
  styleUrl: './doctor-dashboard.component.css'
})
export class DoctorDashboardComponent {
  faAddressCard=faAddressCard;
  faCalendarCheck=faCalendarCheck;
  faNotesMedical=faNotesMedical;
  faRightFromBracket=faRightFromBracket;
  faFile=faFile;
 DoctorObj:any = {
    "doctorId":0 ,
    "userId": 0,
    "fullName": "" ,
    "specialty": "",
    "experience": "",
    "qualification": "",
    "designation": "",
    "contactNumber":""
    }
  isSidebarOpen: boolean = false;
  isAdding: boolean = false;
  
  toggleForm() {
    this.isAdding = !this.isAdding; // Toggle the form
  }
  addAppointment() {
    this.isAdding = true;
    this.resetForm();
  }
  // onEdit(data : any){
  //   this.isAdding = true;
  //   this.DoctorObj=data;
  // }
  onEdit(data : any){
    this.DoctorObj=data;
  }
  toggleSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
  Username: string = '';
  role: string = '';
  UserId: number = 0; 
  userList:any [] = [];
  doctorId: number = 0;
  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    const token = localStorage.getItem('authToken');
    if (token) {
      const decoded = jwtDecode<{ [key: string]: any }>(token);
      this.Username = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '';
      this.role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || '';
      this.UserId = parseInt(decoded["UserId"], 10);
      
    }
  }
// fetchData(){
//   const UserId = this.UserId;
//   console.log("Doctor ID: ", UserId);
//   const token = localStorage.getItem('authToken');
//     const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
//   this.http.get(`https://localhost:7159/api/Doctor/${UserId}`, { headers }).subscribe((data:any)=>{
//     console.log(data);
//     this.DoctorObj = data;
//   });
// }
getAppointmentById(doctorId: number) {
  const token = localStorage.getItem('authToken');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  this.http.get(`https://localhost:7159/api/Doctor/${doctorId}`, { headers }).subscribe((result: any) => {
    this.DoctorObj = result;
    console.log('User appointments:', result);
    alert("Thank you, Here is your profile!");
  },
  (error: any) => {
    if (error.status === 404) {
        alert("You can only access your own Appointments.");
    }
});
}
onSubmit(){
  const token = localStorage.getItem('authToken');  // Retrieve the token
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  this.http.post("https://localhost:7159/api/Doctor",this.DoctorObj, { headers }).subscribe((res:any)=>{
    console.log('Profile added successfully:', res);
    //this.getAllUser(); // Refresh the list of appointments
    this.isAdding = false; 
    this.resetForm(); // Reset the form fields
    this.doctorId = res.doctorId; 
    alert(`Profile successfully created. Your Doctor ID is: ${this.doctorId}`);
  })
}
onUpdate(){
  const doctorId = this.DoctorObj.doctorId;
  const token = localStorage.getItem('authToken');  // Retrieve the token
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  this.http.put(`https://localhost:7159/api/Doctor/${doctorId}`,this.DoctorObj, { headers }).subscribe((res:any)=>{
    console.log('Profile updated successfully:', res);
    //this.getAllUser(); // Refresh the list of appointments
    this.isAdding = false; 
    this.resetForm(); // Reset the form fields
    alert('Profile updated successfully!');
  },
  (error: any) => {
    if (error.status === 403) {
        alert("You cannot update this Profile. ");
    }
}
)
}
  

  logout(): void {
    localStorage.removeItem('authToken'); // Remove the token
    this.router.navigate(['/app-login']); // Redirect to the login page
  }
  resetForm() {
    this.DoctorObj = {
      doctorId: 0,
    userId: 0,
    fullName: "" ,
    specialty: "",
    experience: "",
    qualification: "",
    designation: "",
    contactNumber:""
    };
  }
}
