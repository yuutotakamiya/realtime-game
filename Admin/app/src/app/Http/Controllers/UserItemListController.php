<?php

namespace App\Http\Controllers;


use App\Models\User;
use App\Models\Useritem;
use Barryvdh\Debugbar\Facades\Debugbar;
use Illuminate\Http\Request;

class UserItemListController extends Controller
{
    //所持アイテム一覧を表示する
    public function Useritem(Request $request)
    {
        // プレイヤーテーブルとアイテムテーブルと所持個数を結合して取得
        //$playerItems = User_item::join('users', 'user_items.id', '=', 'users.id')
        //->join('items', 'user_items.id', '=', 'items.id')
        //->select('user_items.id', 'users.user_name as user_name', 'items.item_name',
        //'user_items.Quantity_in_possession')
        //->get();

        $user = User::find($request->id);
        if (!empty($user)) {
            $items = $user->items()->paginate(10);
            $items->appends(['id' => $request->id]);
        }
        return view('users.useritemList', ['user' => $user, 'items' => $items ?? null]);
    }
}
