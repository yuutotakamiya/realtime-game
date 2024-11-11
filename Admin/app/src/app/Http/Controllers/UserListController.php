<?php

namespace App\Http\Controllers;

use App\Models\User;
use Barryvdh\Debugbar\Facades\Debugbar;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Validator;

class UserListController extends Controller
{
    public function UserList(Request $request)
    {
        //$Players = User::all();
        $Players = User::Paginate(10);
        return view('users.userList', ['accounts' => $Players]);
    }
}
