<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class gameManagementController extends Controller
{
    public function gameManagement(Request $request)
    {
        return view('accounts/gameManagement');
    }
}
