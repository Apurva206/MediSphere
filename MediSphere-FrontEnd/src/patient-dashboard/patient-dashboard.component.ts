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
  selector: 'app-patient-dashboard',
  imports: [RouterLink,RouterLinkActive,RouterOutlet,CommonModule,FormsModule, FontAwesomeModule],
  templateUrl: './patient-dashboard.component.html',
  styleUrl: './patient-dashboard.component.css'
})
export class PatientDashboardComponent {
  faAddressCard=faAddressCard;
  faCalendarCheck=faCalendarCheck;
  faNotesMedical=faNotesMedical;
  faRightFromBracket=faRightFromBracket;
  faFile=faFile;
  DoctorObj:any = {
    "patientId":0 ,
    "userId": 0,
    "fullName": "" ,
    "dateOfBirth": "",
    "gender": "",
    "contactNumber": "",
    "address": "",
    "medicalHistory":""
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
  onEdit(data : any){
    this.DoctorObj=data;
  }

  toggleSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
  Username: string = '';
  Role: string = '';
  UserId: number = 0; 
  userList:any [] = [];
  patientId: number = 0;
  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    const token = localStorage.getItem('authToken');
    if (token) {
      const decoded = jwtDecode<{ [key: string]: any }>(token);
      this.Username = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '';
      this.Role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || '';
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
getAppointmentById(patientId: number) {
  const token = localStorage.getItem('authToken');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  this.http.get(`https://localhost:7159/api/Patient/${patientId}`, { headers }).subscribe((result: any) => {
    this.DoctorObj = result;
    console.log('User appointments:', result);
    alert("Thank you, Here is your profile!");
  },
  (error: any) => {
    if (error.status === 404) {
        alert("You can only access your own Profile.");
    }
});
}
onSubmit(){
  const token = localStorage.getItem('authToken');  // Retrieve the token
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  this.http.post("https://localhost:7159/api/Patient",this.DoctorObj, { headers }).subscribe((res:any)=>{
    console.log('Profile added successfully:', res);
    //this.getAllUser(); // Refresh the list of appointments
    this.isAdding = false; 
    this.resetForm(); // Reset the form fields
    this.patientId = res.patientId; 
    alert(`Profile successfully created. Your Patient ID is: ${this.patientId}`);
  })
}
onUpdate(){
  const patientId = this.DoctorObj.patientId;
  const token = localStorage.getItem('authToken');  // Retrieve the token
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  this.http.put(`https://localhost:7159/api/Patient/${patientId}`,this.DoctorObj, { headers }).subscribe((res:any)=>{
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
    patientId:0 ,
    userId: 0,
    fullName: "" ,
    dateOfBirth: "",
    gender: "",
    contactNumber: "",
    address: "",
    medicalHistory:""
    };
  }
}
